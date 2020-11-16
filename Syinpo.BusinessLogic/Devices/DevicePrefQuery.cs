using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Caches;
using Syinpo.Core.Mapper;
using Syinpo.Model.Dto.Devices;

namespace Syinpo.BusinessLogic.Devices {
     public partial class DeviceService {
         public async Task<DeviceDto> GetDeviceById_Pref( int id ) {
             if( id == 0 )
                 return null;

             return await _cache.HGetSet(
                 CacheKeys.Device_Hash, string.Format( CacheKeys.Device_Hash_Id, id ),
                 async () =>
                 {
                     var wx = await GetDeviceById( id );
                     return wx?.MapTo<DeviceDto>();
                 },
                 60 * 24 );
         }

         public async Task<DeviceDto> GetDeviceByUuid_Pref( string uuid ) {
             if( string.IsNullOrEmpty( uuid ) )
                 return null;

             return await _cache.HGetSet(
                 CacheKeys.Device_Hash, string.Format( CacheKeys.Device_Hash_Uuid, uuid ),
                 async () =>
                 {
                     var wx = await GetDeviceByDeviceUuid( uuid );
                     return wx?.MapTo<DeviceDto>();
                 },
                 60 * 24 );
         }
    }
}
