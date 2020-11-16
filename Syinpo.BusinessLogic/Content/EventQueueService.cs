using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syinpo.Core;
using Syinpo.Core.Data;
using Syinpo.Core.Domain.Poco;

namespace Syinpo.BusinessLogic.Content {

    public partial class EventQueueService : IEventQueueService
    {
        #region 字段

        private readonly IDbContext _dbContext;
        private readonly IGenericRepository<EventQueue> _eventQueueRepository;

        #endregion

        #region 构造函数

        public EventQueueService( IDbContext dbContext, IGenericRepository<EventQueue> eventQueueRepository ) {
            _dbContext = dbContext;
            _eventQueueRepository = eventQueueRepository;
        }

        #endregion

        #region 方法

        public async Task InsertEventQueue( EventQueue eventQueue ) {
            if( eventQueue == null )
                throw new ArgumentNullException( nameof( eventQueue ) );

            await _eventQueueRepository.Create( eventQueue );
        }

        public async Task InsertRangeEventQueue( List<EventQueue> eventQueues ) {
            if( eventQueues == null )
                throw new ArgumentNullException( nameof( eventQueues ) );

            await _eventQueueRepository.CreateRange( eventQueues );
        }

        public async Task UpdateEventQueue( EventQueue eventQueue ) {
            if( eventQueue == null )
                throw new ArgumentNullException( nameof( eventQueue ) );

            await _eventQueueRepository.Update( eventQueue );
        }
        public async Task UpdateRangeEventQueue( List<EventQueue> eventQueues ) {
            if( eventQueues == null )
                throw new ArgumentNullException( nameof( eventQueues ) );

            await _eventQueueRepository.UpdateRange( eventQueues );
        }

        public async Task DeleteEventQueue( EventQueue eventQueue ) {
            if( eventQueue == null )
                throw new ArgumentNullException( nameof( eventQueue ) );

            await _eventQueueRepository.Delete( eventQueue );
        }

        public async Task DeleteRangeEventQueue( List<EventQueue> eventQueues ) {
            if( eventQueues == null )
                throw new ArgumentNullException( nameof( eventQueues ) );

            await _eventQueueRepository.DeleteRange( eventQueues );
        }

        public async Task<EventQueue> GetEventQueueById( int eventQueueId ) {
            if( eventQueueId == 0 )
                return null;

            return await _eventQueueRepository.GetById( eventQueueId );
        }

        public async Task<PageList<EventQueue>> SearchEventQueues(
            string version = null, string routeName = null,
            bool onlyNoOut = true,
            int maxRetry = 3, int pageIndex = 0, int pageSize = 100000 ) {
            var query = _eventQueueRepository.Table;

            if( !string.IsNullOrEmpty( version ) )
                query = query.Where( q => q.Version == version );

            if( !string.IsNullOrEmpty( routeName ) )
                query = query.Where( q => q.RouteName == routeName );

            if( onlyNoOut )
                query = query.Where( q => !q.OutTime.HasValue );

            query = query.Where( q => q.Retry < maxRetry );
            query = query.OrderBy( q => q.Id );

            var eventQueues = new PageList<EventQueue>( query, pageIndex, pageSize );
            return await Task.FromResult( eventQueues );
        }


        #endregion
    }
}
