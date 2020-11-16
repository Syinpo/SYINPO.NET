using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Caches;
using Syinpo.Core;
using Syinpo.Core.Caches;
using Syinpo.Core.Data;
using Syinpo.Core.Domain.MonitorPoco;
using Syinpo.Core.Domain.Poco;
using Syinpo.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Syinpo.BusinessLogic.Monitor {
   public  class RequestLogService : IRequestLogService
   {
        #region 字段

        private readonly IDbContext _dbContext;
        private readonly IGenericRepository<RequestLog> _requestLogRepository;
        private readonly ICacheEvent _cacheEvent;
        private readonly ICache _cache;

        #endregion

        #region 构造函数

        public RequestLogService(
            IDbContext dbContext,
            IGenericRepository<RequestLog> requestLogRepository,
            ICache cache,
            ICacheEvent cacheEvent ) {
            this._dbContext = dbContext;
            this._requestLogRepository = requestLogRepository;
            this._cacheEvent = cacheEvent;
            this._cache = cache;
        }

        #endregion

        #region 方法

        #region RequestLogs

        public async Task<PageList<RequestLog>> SearchRequestLogs( DateTime? createTimeStart = null, DateTime? createTimeEnd = null,  string keywords = null, int pageIndex = 0, int pageSize = 10000 ) {
            var query = _requestLogRepository.Table;
            if( createTimeStart.HasValue && createTimeStart.Value.Year > 1 )
                query = query.Where( c => createTimeStart.Value <= c.CreateTime );
            if( createTimeEnd.HasValue && createTimeEnd.Value.Year > 1 )
                query = query.Where( c => createTimeEnd.Value >= c.CreateTime );

            if( !string.IsNullOrEmpty( keywords ) )
                query = query.Where( c => c.RequestBody.Contains( keywords ) );


            query = query.AsNoTracking().OrderByDescending( c => c.Id );

            var requestLogs = new PageList<RequestLog>( query, pageIndex, pageSize );

            return await Task.FromResult( requestLogs );
        }


        public async Task DeleteRequestLog( RequestLog requestLog ) {
            if( requestLog == null )
                throw new ArgumentNullException( nameof( requestLog ) );

            await _requestLogRepository.Delete(requestLog);
        }

        public async Task<RequestLog> GetRequestLogById( int requestLogId ) {
            if( requestLogId == 0 )
                return null;

            return await _requestLogRepository.GetById( requestLogId );
        }


        public async Task<List<RequestLog>> GetRequestLogsByIds( int[] requestLogIds ) {
            if( requestLogIds == null || requestLogIds.Length == 0 )
                return new List<RequestLog>();

            var query = from c in _requestLogRepository.Table
                        where requestLogIds.Contains( c.Id )
                        select c;
            var requestLogs = query.ToList();

            return await Task.FromResult( requestLogs );
        }

        public async Task<RequestLog> GetRequestLogByTraceId( string traceId ) {
            if( string.IsNullOrWhiteSpace( traceId ) )
                return null;

            var requestLog = await _requestLogRepository.Table
                .Where( x =>   x.TraceId == traceId )
                .FirstOrDefaultAsync();
            return requestLog;
        }

        public async Task InsertRequestLog( RequestLog requestLog ) {
            if( requestLog == null )
                throw new ArgumentNullException( nameof( requestLog ) );

            await _requestLogRepository.Create( requestLog );
        }

        public async Task UpdateRequestLog( RequestLog requestLog ) {
            if( requestLog == null )
                throw new ArgumentNullException( nameof( requestLog ) );

            await _requestLogRepository.Update( requestLog );
        }


        #endregion

        #endregion
    }
}
