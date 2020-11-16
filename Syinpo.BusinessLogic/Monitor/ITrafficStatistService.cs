using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syinpo.Core;
using Syinpo.Core.Domain.MonitorPoco;

namespace Syinpo.BusinessLogic.Monitor
{
    public interface ITrafficStatistService
    {
        Task<PageList<TrafficStatist>> SearchTrafficStatists( DateTime? createTimeStart = null, DateTime? createTimeEnd = null, string keywords = null, int pageIndex = 0, int pageSize = 10000 );
        Task DeleteTrafficStatist( TrafficStatist trafficStatist );
        Task<TrafficStatist> GetTrafficStatistById( int trafficStatistId );
        Task<List<TrafficStatist>> GetTrafficStatistsByIds( int[] trafficStatistIds );
        Task InsertTrafficStatist( TrafficStatist trafficStatist );
        Task UpdateTrafficStatist( TrafficStatist trafficStatist );
        Task<TrafficStatist> GetTrafficStatistByDateTime(long dateTime);
    }
}