using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syinpo.Core;
using Syinpo.Core.Domain.MonitorPoco;

namespace Syinpo.BusinessLogic.Monitor {
    public interface IResponseLogService {
        Task<PageList<ResponseSnap>> SearchResponseSnaps( DateTime? createTimeStart = null, DateTime? createTimeEnd = null, string keywords = null, int pageIndex = 0, int pageSize = 10000 );
        Task DeleteResponseSnap( ResponseSnap responseSnap );
        Task<ResponseSnap> GetResponseSnapById( int responseSnapId );
        Task<List<ResponseSnap>> GetResponseSnapsByIds( int[] responseSnapIds );
        Task<ResponseSnap> GetResponseSnapByTraceId( string traceId );
        Task InsertResponseSnap( ResponseSnap responseSnap );
        Task UpdateResponseSnap( ResponseSnap responseSnap );
        Task<PageList<SqlSnap>> SearchSqlSnaps( DateTime? createTimeStart = null, DateTime? createTimeEnd = null, string keywords = null, int pageIndex = 0, int pageSize = 10000 );
        Task DeleteSqlSnap( SqlSnap sqlSnap );
        Task<SqlSnap> GetSqlSnapById( int sqlSnapId );
        Task<List<SqlSnap>> GetSqlSnapsByIds( int[] sqlSnapIds );
        Task<List<SqlSnap>> GetSqlSnapsByTraceId( string traceId );
        Task InsertSqlSnap( SqlSnap sqlSnap );
        Task UpdateSqlSnap( SqlSnap sqlSnap );
        Task<PageList<ExceptionSnap>> SearchExceptionSnaps( DateTime? createTimeStart = null, DateTime? createTimeEnd = null, string keywords = null, int pageIndex = 0, int pageSize = 10000 );
        Task DeleteExceptionSnap( ExceptionSnap exceptionSnap );
        Task<ExceptionSnap> GetExceptionSnapById( int exceptionSnapId );
        Task<List<ExceptionSnap>> GetExceptionSnapsByIds( int[] exceptionSnapIds );
        Task<List<ExceptionSnap>> GetExceptionSnapsByTraceId( string traceId );
        Task InsertExceptionSnap( ExceptionSnap exceptionSnap );
        Task UpdateExceptionSnap( ExceptionSnap exceptionSnap );

        Task<PageList<TrackSnap>> SearchTrackSnaps( DateTime? createTimeStart = null, DateTime? createTimeEnd = null, string keywords = null, int pageIndex = 0, int pageSize = 10000 );

        Task DeleteTrackSnap( TrackSnap trackSnap );

        Task<TrackSnap> GetTrackSnapById( int trackSnapId );

        Task<List<TrackSnap>> GetTrackSnapsByIds( int[] trackSnapIds );

        Task<List<TrackSnap>> GetTrackSnapsByTraceId( string traceId );

        Task InsertTrackSnap( TrackSnap trackSnap );

        Task UpdateTrackSnap( TrackSnap trackSnap );
    }
}