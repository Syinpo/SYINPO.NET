using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Caches;
using Syinpo.Core;
using Syinpo.Core.Caches;
using Syinpo.Core.Data;
using Syinpo.Core.Domain.Poco;
using Syinpo.Model.ViewResult.Devices;
using Syinpo.Core.EasyLicense.License.Validator;
using Syinpo.Core.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Syinpo.Core.Mapper;
using Syinpo.Model.Dto.Devices;
using Syinpo.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Reflection;
using EFCoreSecondLevelCacheInterceptor;
using Newtonsoft.Json;

namespace Syinpo.BusinessLogic.Devices {
    public partial class DeviceService : IDeviceService
    {
        private readonly IGenericRepository<Device> _deviceRepository;
        private readonly IGenericRepository<DeviceSms> _deviceSmsRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly ICacheEvent _cacheEvent;
        private readonly ICache _cache;
        private readonly ILogger<DeviceService> _logger;
        private readonly IDbContext _dbContext;

        public DeviceService(
              IGenericRepository<Device> deviceRepository,
              IGenericRepository<DeviceSms> deviceSmsRepository,
              IGenericRepository<User> userRepository,
              IDbContext dbContext,
              ICache cache,
              ICacheEvent cacheEvent,
              ILogger<DeviceService> logger ) {
            _deviceRepository = deviceRepository;
            _deviceSmsRepository = deviceSmsRepository;
            _userRepository = userRepository;
            _logger = logger;
            _cacheEvent = cacheEvent;
            _cache = cache;
            _dbContext = dbContext;
        }


        #region Device

        public async Task DeleteDevice( Device device ) {
            if( device == null )
                throw new ArgumentNullException( nameof( device ) );

            device.Deleted = true;
            device.DeviceUuid = device.DeviceUuid + "-del-" + DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss" );
            device.UpdateTime = DateTime.Now;
            await UpdateDevice( device );
        }

        public async Task<PageList<Device>> SearchDevices( List<int> deviceIds = null, int? deviceGroupId = null, bool? excludePostDevice = null, bool? isOnLine = null, int? ListType = null, string keywords = null, bool? orderSms = null, int pageIndex = 0, int pageSize = int.MaxValue, DateTime? startTime = null, DateTime? endTime = null ) {
            var query = _deviceRepository.Table.Where( w => !w.Deleted )
                .WhereIF( isOnLine.HasValue, w => w.IsOnline == isOnLine.Value )
                .WhereIF( ListType.HasValue, w => w.ListType == ListType.Value );

            if( deviceIds != null ) {
                if( deviceIds.Any() )
                    query = query.Where( w => deviceIds.Contains( w.Id ) );
                else
                    query = query.Where( w => w.Id == 0 );
            }

            return await Task.FromResult( new PageList<Device>( query, pageIndex, pageSize ) );
        }


        public async Task<Device> GetDeviceById( int deviceId ) {
            if( deviceId == 0 )
                return null;

            return await _deviceRepository.GetById( deviceId );
        }


        public async Task<List<Device>> GetDeviceListByIds( int[] deviceIds ) {
            if( deviceIds == null )
                return null;

            var devices = _deviceRepository.Table.Where( x => !x.Deleted && deviceIds.Contains( x.Id ) ).ToList();
            return await Task.FromResult( devices );
        }

        public async Task<Device> GetDeviceByDeviceUuid( string uuid ) {
            if( string.IsNullOrEmpty( uuid ) )
                return null;

            var device = _deviceRepository.Table.FirstOrDefault( f => f.DeviceUuid == uuid );

            return await Task.FromResult( device );
        }


        public async Task<Device> InsertGuestDevice( string deviceUuid ) {
            var device = new Device {
                TrackingId = CommonHelper.NewSequentialGuid().ToString(),
                DeviceUuid = deviceUuid,
                Approved = false,
                Deleted = false,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };

            await InsertDevice( device );

            return device;
        }

        public async Task InsertDevice( Device device ) {
            if( device == null )
                throw new ArgumentNullException( nameof( device ) );

            await _deviceRepository.Create( device );

            _cacheEvent.Publish( device, ChangeType.Insert );
        }


        public async Task UpdateDevice( Device device ) {
            if( device == null )
                throw new ArgumentNullException( nameof( device ) );

            await _deviceRepository.Update( device );
            _cacheEvent.Publish( device, ChangeType.Update );
        }

        public async Task SetListType( int[] deviceIds, int listType ) {
            if( deviceIds.Count() < 1 )
                return;

            if( !new List<int>() { 0, 1, 2 }.Contains( listType ) )
                return;

            var deviceList = await _deviceRepository.Table
                .Where( a => deviceIds.Contains( a.Id ) ).ToListAsync();
            foreach( var item in deviceList ) {
                item.ListType = listType;

            }

            await _deviceRepository.UpdateRange( deviceList );
            foreach( var item in deviceList ) {
                _cacheEvent.Publish( item, ChangeType.Update );
            }

        }

        #endregion

        #region DeviceSms

        /// <summary>
        ///
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="keywords"></param>
        /// <param name="sent"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="filterSensitiveWord">过滤敏感词 （ 0 全部，1 有敏感词，2 没有敏感词 ）</param>
        /// <param name="sensitiveWord">敏感词</param>
        /// <returns></returns>
        public async Task<PageList<DeviceSms>> SearchDeviceSms( int? deviceId = 0, string keywords = null, bool? sent = null, DateTime? startTime = null, DateTime? endTime = null, int pageIndex = 0, int pageSize = 10000, int filterSensitiveWord = 0, string sensitiveWord = null ) {
            var query = _deviceSmsRepository.Table
                .WhereIF( deviceId.HasValue && deviceId > 0, w => w.DeviceId == deviceId )
                .WhereIF( !string.IsNullOrEmpty( keywords ), w => w.FromPhone.Contains( keywords ) || w.ToPhone.Contains( keywords ) )
                .WhereIF( sent.HasValue, w => w.Sent == sent )
                .WhereIF( startTime.HasValue, w => w.SmsCreateTime >= startTime.Value )
                .WhereIF( endTime.HasValue, w => w.SmsCreateTime < endTime.Value )
                .WhereIF( filterSensitiveWord > 0 && filterSensitiveWord == 1, a => !string.IsNullOrWhiteSpace( a.SensitiveWord ) )
                 .WhereIF( filterSensitiveWord > 0 && filterSensitiveWord == 2, a => string.IsNullOrWhiteSpace( a.SensitiveWord ) )
                 .WhereIF( !string.IsNullOrWhiteSpace( sensitiveWord ), a => a.SensitiveWord.Contains( sensitiveWord ) );

            query = query.OrderByDescending( p => p.SmsCreateTime );
            var deviceSms = new PageList<DeviceSms>( query, pageIndex, pageSize );
            return await Task.FromResult( deviceSms );

        }


        public async Task DeleteDeviceSms( DeviceSms deviceSms ) {
            if( deviceSms == null )
                throw new ArgumentNullException( nameof( deviceSms ) );

            await _deviceSmsRepository.Delete( deviceSms );
        }

        public async Task<DeviceSms> GetDeviceSmsById( int id ) {
            if( id == 0 )
                return null;

            return await _deviceSmsRepository.GetById( id );
        }


        public async Task<List<DeviceSms>> GetDeviceSmsByIds( int[] deviceSmsIds ) {
            if( deviceSmsIds == null || deviceSmsIds.Length == 0 )
                return new List<DeviceSms>();

            var query = from c in _deviceSmsRepository.Table
                        where deviceSmsIds.Contains( c.Id )
                        select c;
            var deviceSms = query.ToList();

            return await Task.FromResult( deviceSms );
        }

        public async Task InsertDeviceSms( DeviceSms deviceSms ) {
            if( deviceSms == null )
                throw new ArgumentNullException( nameof( deviceSms ) );

            await _deviceSmsRepository.Create( deviceSms );
        }

        public async Task InsertDeviceSmsRange( List<DeviceSms> deviceSms ) {
            if( deviceSms == null )
                throw new ArgumentNullException( nameof( deviceSms ) );

            await _deviceSmsRepository.CreateRange( deviceSms );
        }

        public async Task UpdateDeviceSms( DeviceSms deviceSms ) {
            if( deviceSms == null )
                throw new ArgumentNullException( nameof( deviceSms ) );

            await _deviceSmsRepository.Update( deviceSms );
        }


        #endregion
    }
}

