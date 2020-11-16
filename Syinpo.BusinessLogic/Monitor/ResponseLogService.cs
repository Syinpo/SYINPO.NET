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
using Microsoft.EntityFrameworkCore;

namespace Syinpo.BusinessLogic.Monitor {
    public class ResponseLogService : IResponseLogService
    {
        #region 字段

        private readonly IDbContext _dbContext;
        private readonly IGenericRepository<ResponseSnap> _responseSnapRepository;
        private readonly IGenericRepository<SqlSnap> _sqlSnapRepository;
        private readonly IGenericRepository<ExceptionSnap> _exceptionSnapRepository;
        private readonly IGenericRepository<TrackSnap> _trackSnapRepository;
        private readonly ICacheEvent _cacheEvent;
        private readonly ICache _cache;

        #endregion

        #region 构造函数

        public ResponseLogService(
            IDbContext dbContext,
            IGenericRepository<ResponseSnap> responseSnapRepository,
            IGenericRepository<SqlSnap> sqlSnapRepository,
            IGenericRepository<ExceptionSnap> exceptionSnapRepository,
            IGenericRepository<TrackSnap> trackSnapRepository,
            ICache cache,
            ICacheEvent cacheEvent ) {
            this._dbContext = dbContext;
            this._responseSnapRepository = responseSnapRepository;
            this._sqlSnapRepository = sqlSnapRepository;
            this._exceptionSnapRepository = exceptionSnapRepository;
            this._trackSnapRepository = trackSnapRepository;
            this._cacheEvent = cacheEvent;
            this._cache = cache;
        }

        #endregion

        #region 方法

        #region ResponseSnaps

        public async Task<PageList<ResponseSnap>> SearchResponseSnaps( DateTime? createTimeStart = null, DateTime? createTimeEnd = null, string keywords = null, int pageIndex = 0, int pageSize = 10000 ) {
            var query = _responseSnapRepository.Table;
            if( createTimeStart.HasValue && createTimeStart.Value.Year > 1 )
                query = query.Where( c => createTimeStart.Value <= c.CreateTime );
            if( createTimeEnd.HasValue && createTimeEnd.Value.Year > 1 )
                query = query.Where( c => createTimeEnd.Value >= c.CreateTime );

            if( !string.IsNullOrEmpty( keywords ) )
                query = query.Where( c => c.ResponseBody.Contains( keywords ) );


            query = query.OrderBy( c => c.Id );

            var responseSnaps = new PageList<ResponseSnap>( query, pageIndex, pageSize );

            return await Task.FromResult( responseSnaps );
        }


        public async Task DeleteResponseSnap( ResponseSnap responseSnap ) {
            if( responseSnap == null )
                throw new ArgumentNullException( nameof( responseSnap ) );

            await _responseSnapRepository.Delete( responseSnap );
        }

        public async Task<ResponseSnap> GetResponseSnapById( int responseSnapId ) {
            if( responseSnapId == 0 )
                return null;

            return await _responseSnapRepository.GetById( responseSnapId );
        }


        public async Task<List<ResponseSnap>> GetResponseSnapsByIds( int[] responseSnapIds ) {
            if( responseSnapIds == null || responseSnapIds.Length == 0 )
                return new List<ResponseSnap>();

            var query = from c in _responseSnapRepository.Table
                        where responseSnapIds.Contains( c.Id )
                        select c;
            var responseSnaps = query.ToList();

            return await Task.FromResult( responseSnaps );
        }

        public async Task<ResponseSnap> GetResponseSnapByTraceId( string traceId ) {
            if( string.IsNullOrWhiteSpace( traceId ) )
                return null;

            var responseSnap = await _responseSnapRepository.Table
                .Where( x => x.TraceId == traceId )
                .FirstOrDefaultAsync();
            return responseSnap;
        }

        public async Task InsertResponseSnap( ResponseSnap responseSnap ) {
            if( responseSnap == null )
                throw new ArgumentNullException( nameof( responseSnap ) );

            await _responseSnapRepository.Create( responseSnap );
        }

        public async Task UpdateResponseSnap( ResponseSnap responseSnap ) {
            if( responseSnap == null )
                throw new ArgumentNullException( nameof( responseSnap ) );

            await _responseSnapRepository.Update( responseSnap );
        }


        #endregion

        #region SqlSnaps

        public async Task<PageList<SqlSnap>> SearchSqlSnaps( DateTime? createTimeStart = null, DateTime? createTimeEnd = null, string keywords = null, int pageIndex = 0, int pageSize = 10000 ) {
            var query = _sqlSnapRepository.Table;
            if( createTimeStart.HasValue && createTimeStart.Value.Year > 1 )
                query = query.Where( c => createTimeStart.Value <= c.CreateTime );
            if( createTimeEnd.HasValue && createTimeEnd.Value.Year > 1 )
                query = query.Where( c => createTimeEnd.Value >= c.CreateTime );

            if( !string.IsNullOrEmpty( keywords ) )
                query = query.Where( c => c.SqlRaw.Contains( keywords ) );


            query = query.OrderBy( c => c.Id );

            var sqlSnaps = new PageList<SqlSnap>( query, pageIndex, pageSize );

            return await Task.FromResult( sqlSnaps );
        }


        public async Task DeleteSqlSnap( SqlSnap sqlSnap ) {
            if( sqlSnap == null )
                throw new ArgumentNullException( nameof( sqlSnap ) );

            await _sqlSnapRepository.Delete( sqlSnap );
        }

        public async Task<SqlSnap> GetSqlSnapById( int sqlSnapId ) {
            if( sqlSnapId == 0 )
                return null;

            return await _sqlSnapRepository.GetById( sqlSnapId );
        }


        public async Task<List<SqlSnap>> GetSqlSnapsByIds( int[] sqlSnapIds ) {
            if( sqlSnapIds == null || sqlSnapIds.Length == 0 )
                return new List<SqlSnap>();

            var query = from c in _sqlSnapRepository.Table
                        where sqlSnapIds.Contains( c.Id )
                        select c;
            var sqlSnaps = query.ToList();

            return await Task.FromResult( sqlSnaps );
        }

        public async Task<List<SqlSnap>> GetSqlSnapsByTraceId( string traceId ) {
            if( string.IsNullOrWhiteSpace( traceId ) )
                return null;

            var sqlSnaps = await _sqlSnapRepository.Table
                .Where( x => x.TraceId == traceId )
                .ToListAsync();
            return sqlSnaps;
        }

        public async Task InsertSqlSnap( SqlSnap sqlSnap ) {
            if( sqlSnap == null )
                throw new ArgumentNullException( nameof( sqlSnap ) );

            await _sqlSnapRepository.Create( sqlSnap );
        }

        public async Task UpdateSqlSnap( SqlSnap sqlSnap ) {
            if( sqlSnap == null )
                throw new ArgumentNullException( nameof( sqlSnap ) );

            await _sqlSnapRepository.Update( sqlSnap );
        }


        #endregion

        #region ExceptionSnaps

        public async Task<PageList<ExceptionSnap>> SearchExceptionSnaps( DateTime? createTimeStart = null, DateTime? createTimeEnd = null, string keywords = null, int pageIndex = 0, int pageSize = 10000 ) {
            var query = _exceptionSnapRepository.Table;
            if( createTimeStart.HasValue && createTimeStart.Value.Year > 1 )
                query = query.Where( c => createTimeStart.Value <= c.CreateTime );
            if( createTimeEnd.HasValue && createTimeEnd.Value.Year > 1 )
                query = query.Where( c => createTimeEnd.Value >= c.CreateTime );

            if( !string.IsNullOrEmpty( keywords ) )
                query = query.Where( c => c.ErrorDetail.Contains( keywords ) );


            query = query.OrderBy( c => c.Id );

            var exceptionSnaps = new PageList<ExceptionSnap>( query, pageIndex, pageSize );

            return await Task.FromResult( exceptionSnaps );
        }


        public async Task DeleteExceptionSnap( ExceptionSnap exceptionSnap ) {
            if( exceptionSnap == null )
                throw new ArgumentNullException( nameof( exceptionSnap ) );

            await _exceptionSnapRepository.Delete( exceptionSnap );
        }

        public async Task<ExceptionSnap> GetExceptionSnapById( int exceptionSnapId ) {
            if( exceptionSnapId == 0 )
                return null;

            return await _exceptionSnapRepository.GetById( exceptionSnapId );
        }


        public async Task<List<ExceptionSnap>> GetExceptionSnapsByIds( int[] exceptionSnapIds ) {
            if( exceptionSnapIds == null || exceptionSnapIds.Length == 0 )
                return new List<ExceptionSnap>();

            var query = from c in _exceptionSnapRepository.Table
                        where exceptionSnapIds.Contains( c.Id )
                        select c;
            var exceptionSnaps = query.ToList();

            return await Task.FromResult( exceptionSnaps );
        }

        public async Task<List<ExceptionSnap>> GetExceptionSnapsByTraceId( string traceId ) {
            if( string.IsNullOrWhiteSpace( traceId ) )
                return null;

            var exceptionSnaps = await _exceptionSnapRepository.Table
                .Where( x => x.TraceId == traceId )
                .ToListAsync();
            return exceptionSnaps;
        }

        public async Task InsertExceptionSnap( ExceptionSnap exceptionSnap ) {
            if( exceptionSnap == null )
                throw new ArgumentNullException( nameof( exceptionSnap ) );

            await _exceptionSnapRepository.Create( exceptionSnap );
        }

        public async Task UpdateExceptionSnap( ExceptionSnap exceptionSnap ) {
            if( exceptionSnap == null )
                throw new ArgumentNullException( nameof( exceptionSnap ) );

            await _exceptionSnapRepository.Update( exceptionSnap );
        }


        #endregion

        #region TrackSnaps

        public async Task<PageList<TrackSnap>> SearchTrackSnaps( DateTime? createTimeStart = null, DateTime? createTimeEnd = null, string keywords = null, int pageIndex = 0, int pageSize = 10000 ) {
            var query = _trackSnapRepository.Table;
            if( createTimeStart.HasValue && createTimeStart.Value.Year > 1 )
                query = query.Where( c => createTimeStart.Value <= c.CreateTime );
            if( createTimeEnd.HasValue && createTimeEnd.Value.Year > 1 )
                query = query.Where( c => createTimeEnd.Value >= c.CreateTime );

            if( !string.IsNullOrEmpty( keywords ) )
                query = query.Where( c => c.TraceData.Contains( keywords ) );


            query = query.OrderBy( c => c.Id );

            var trackSnaps = new PageList<TrackSnap>( query, pageIndex, pageSize );

            return await Task.FromResult( trackSnaps );
        }


        public async Task DeleteTrackSnap( TrackSnap trackSnap ) {
            if( trackSnap == null )
                throw new ArgumentNullException( nameof( trackSnap ) );

            await _trackSnapRepository.Delete( trackSnap );
        }

        public async Task<TrackSnap> GetTrackSnapById( int trackSnapId ) {
            if( trackSnapId == 0 )
                return null;

            return await _trackSnapRepository.GetById( trackSnapId );
        }


        public async Task<List<TrackSnap>> GetTrackSnapsByIds( int[] trackSnapIds ) {
            if( trackSnapIds == null || trackSnapIds.Length == 0 )
                return new List<TrackSnap>();

            var query = from c in _trackSnapRepository.Table
                        where trackSnapIds.Contains( c.Id )
                        select c;
            var trackSnaps = query.ToList();

            return await Task.FromResult( trackSnaps );
        }

        public async Task<List<TrackSnap>> GetTrackSnapsByTraceId( string traceId ) {
            if( string.IsNullOrWhiteSpace( traceId ) )
                return null;

            var trackSnaps = await _trackSnapRepository.Table
                .Where( x => x.TraceId == traceId )
                .ToListAsync();
            return trackSnaps;
        }

        public async Task InsertTrackSnap( TrackSnap trackSnap ) {
            if( trackSnap == null )
                throw new ArgumentNullException( nameof( trackSnap ) );

            await _trackSnapRepository.Create( trackSnap );
        }

        public async Task UpdateTrackSnap( TrackSnap trackSnap ) {
            if( trackSnap == null )
                throw new ArgumentNullException( nameof( trackSnap ) );

            await _trackSnapRepository.Update( trackSnap );
        }


        #endregion

        #endregion
    }
}
