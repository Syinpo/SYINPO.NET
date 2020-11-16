using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syinpo.Core;
using Syinpo.Core.Domain.MonitorPoco;

namespace Syinpo.BusinessLogic.Monitor
{
    public interface IRequestLogService
    {
        Task<PageList<RequestLog>> SearchRequestLogs( DateTime? createTimeStart = null, DateTime? createTimeEnd = null,  string keywords = null, int pageIndex = 0, int pageSize = 10000 );
        Task DeleteRequestLog( RequestLog requestLog );
        Task<RequestLog> GetRequestLogById( int requestLogId );
        Task<List<RequestLog>> GetRequestLogsByIds( int[] requestLogIds );
        Task<RequestLog> GetRequestLogByTraceId( string traceId );
        Task InsertRequestLog( RequestLog requestLog );
        Task UpdateRequestLog( RequestLog requestLog );
    }
}