using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Microsoft.Extensions.Options;
using HangfireBackgroundJob = Hangfire.BackgroundJob;
using HangfireRecurringJob = Hangfire.RecurringJob;

namespace Syinpo.Core.Hangfire {
    public class HangfireBackgroundJobManager : IBackgroundJobManager {


        private readonly HangfireOptions  _hangfireOptions;
        public HangfireBackgroundJobManager(IOptions<HangfireOptions> options)
        {
            _hangfireOptions = options?.Value;
        }

        /// <summary>
        /// 添加一个执行一次的队列Job
        /// 添加一个定时执行一次的队列Job
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <typeparam name="TArgs"></typeparam>
        /// <param name="args"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public Task<string> EnqueueAsync<TJob, TArgs>( TArgs args, TimeSpan? delay = null ) where TJob : IBackgroundJob<TArgs> {
            string jobUniqueIdentifier = string.Empty;
            
            if( !_hangfireOptions.UseHangfire )
                return Task.FromResult( jobUniqueIdentifier );

            if( !delay.HasValue ) {
                jobUniqueIdentifier = HangfireBackgroundJob.Enqueue<TJob>( job => job.Execute( args ) );
            }
            else {
                jobUniqueIdentifier = HangfireBackgroundJob.Schedule<TJob>( job => job.Execute( args ), delay.Value );
            }

            return Task.FromResult( jobUniqueIdentifier );
        }

        /// <summary>
        /// 删除一个队列Job
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public Task<bool> DeleteAsync( string jobId ) {
            if( string.IsNullOrWhiteSpace( jobId ) ) {
                throw new ArgumentNullException( nameof( jobId ) );
            }

            bool successfulDeletion = HangfireBackgroundJob.Delete( jobId );
            return Task.FromResult( successfulDeletion );
        }

        /// <summary>
        /// 查询Job详情
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public Task<JobDetailsDto> FindAsync( string jobId ) {
            if( string.IsNullOrWhiteSpace( jobId ) ) {
                throw new ArgumentNullException( nameof( jobId ) );
            }


            return Task.FromResult( JobStorage.Current.GetMonitoringApi().JobDetails( jobId ) );
        }

        /// <summary>
        /// 查询周期Job详情
        /// </summary>
        /// <param name="recurringJobId"></param>
        /// <returns></returns>
        public Task<RecurringJobDto> FindRecurringJobAsync( string recurringJobId ) {
            if( string.IsNullOrWhiteSpace( recurringJobId ) ) {
                throw new ArgumentNullException( nameof( recurringJobId ) );
            }

            using( var connection = JobStorage.Current.GetConnection() ) {
                var recurringJob = connection.GetRecurringJobs().FirstOrDefault( f => f.Id == recurringJobId );
                return Task.FromResult( recurringJob );
            }
        }

        /// <summary>
        /// 添加与更新周期Job
        /// 这个方法适合给泛型执行的Job使用，如果是独立周期Job，建议在Syinpo.Unity.Hangfire.HangfireRegister中维护。
        /// 泛型使用场景: 轮询发送朋友圈功能，后台两次新增任务
        ///     recurringJobId : 发送朋友圈_SendFriendCircle_Id1
        ///     recurringJobId : 发送朋友圈_SendFriendCircle_Id2
        ///
        ///     TJob :  SendFriendCircleJob.cs
        ///
        ///     TArgs : SendFriendCircleJobArgs.cs { public int Id{get;set;}   public string Other {get;set;} }
        ///
        ///     cronExpression :
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <typeparam name="TArgs"></typeparam>
        /// <param name="recurringJobId"></param>
        /// <param name="args"></param>
        /// <param name="cronExpression"></param>
        /// <returns></returns>
        public Task<string> AddOrUpdateRecurringJobAsync<TJob, TArgs>( string recurringJobId, TArgs args, string cronExpression ) where TJob : IBackgroundJob<TArgs> {
            string jobUniqueIdentifier = string.Empty;
            if( !_hangfireOptions.UseHangfire )
                return Task.FromResult( jobUniqueIdentifier );

            HangfireRecurringJob.AddOrUpdate<TJob>( recurringJobId, a => a.Execute( args ), cronExpression );
            jobUniqueIdentifier = recurringJobId;
            return Task.FromResult( jobUniqueIdentifier );
        }

        /// <summary>
        /// 删除周期性Job
        /// 适合泛型周期Job删除
        /// </summary>
        /// <returns></returns>
        public Task<bool> RemoveRecurringJobAsync( string recurringJobId ) {
            HangfireRecurringJob.RemoveIfExists( recurringJobId );
            return Task.FromResult( true );
        }

        /// <summary>
        /// 删除周期性Job
        /// 不适合泛型周期Job调用
        /// 已标记弃用
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public Task<bool> RemoveRecurringJobAsync<TJob, TArgs>() where TJob : IBackgroundJob<TArgs> {
            string recurringJobId = typeof( TJob ).Name + "." + "Execute";
            HangfireRecurringJob.RemoveIfExists( recurringJobId );
            return Task.FromResult( true );
        }

    }
}
