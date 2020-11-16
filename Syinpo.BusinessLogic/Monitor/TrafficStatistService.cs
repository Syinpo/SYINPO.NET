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
    public class TrafficStatistService : ITrafficStatistService
    {
        #region 字段

        private readonly IDbContext _dbContext;
        private readonly IGenericRepository<TrafficStatist> _trafficStatistRepository;
        private readonly ICacheEvent _cacheEvent;
        private readonly ICache _cache;

        #endregion

        #region 构造函数

        public TrafficStatistService(
            IDbContext dbContext,
            IGenericRepository<TrafficStatist> trafficStatistRepository,
            ICache cache,
            ICacheEvent cacheEvent ) {
            this._dbContext = dbContext;
            this._trafficStatistRepository = trafficStatistRepository;
            this._cacheEvent = cacheEvent;
            this._cache = cache;
        }

        #endregion

        #region 方法

        #region TrafficStatists

        public async Task<PageList<TrafficStatist>> SearchTrafficStatists( DateTime? createTimeStart = null, DateTime? createTimeEnd = null, string keywords = null, int pageIndex = 0, int pageSize = 10000 ) {
            var query = _trafficStatistRepository.Table;
            if( createTimeStart.HasValue && createTimeStart.Value.Year > 1 )
                query = query.Where( c => createTimeStart.Value <= c.CreateTime );
            if( createTimeEnd.HasValue && createTimeEnd.Value.Year > 1 )
                query = query.Where( c => createTimeEnd.Value >= c.CreateTime );

            query = query.OrderBy( c => c.DateTime );

            var trafficStatists = new PageList<TrafficStatist>( query, pageIndex, pageSize );

            return await Task.FromResult( trafficStatists );
        }


        public async Task DeleteTrafficStatist( TrafficStatist trafficStatist ) {
            if( trafficStatist == null )
                throw new ArgumentNullException( nameof( trafficStatist ) );

            await _trafficStatistRepository.Delete( trafficStatist );
        }

        public async Task<TrafficStatist> GetTrafficStatistById( int trafficStatistId ) {
            if( trafficStatistId == 0 )
                return null;

            return await _trafficStatistRepository.GetById( trafficStatistId );
        }

        public async Task<TrafficStatist> GetTrafficStatistByDateTime( long dateTime ) {
            if( dateTime == 0 )
                return null;

            return await _trafficStatistRepository.Table.FirstOrDefaultAsync( f => f.DateTime == dateTime );
        }


        public async Task<List<TrafficStatist>> GetTrafficStatistsByIds( int[] trafficStatistIds ) {
            if( trafficStatistIds == null || trafficStatistIds.Length == 0 )
                return new List<TrafficStatist>();

            var query = from c in _trafficStatistRepository.Table
                        where trafficStatistIds.Contains( c.Id )
                        select c;
            var trafficStatists = query.ToList();

            return await Task.FromResult( trafficStatists );
        }


        public async Task InsertTrafficStatist( TrafficStatist trafficStatist ) {
            if( trafficStatist == null )
                throw new ArgumentNullException( nameof( trafficStatist ) );

            await _trafficStatistRepository.Create( trafficStatist );
        }

        public async Task UpdateTrafficStatist( TrafficStatist trafficStatist ) {
            if( trafficStatist == null )
                throw new ArgumentNullException( nameof( trafficStatist ) );

            await _trafficStatistRepository.Update( trafficStatist );
        }


        #endregion

        #endregion
    }
}
