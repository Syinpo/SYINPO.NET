using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syinpo.Core;
using Syinpo.Core.Domain.Poco;
using Syinpo.Model.Dto.Devices;

namespace Syinpo.BusinessLogic.Devices
{
    public interface IDeviceService
    {
        Task DeleteDevice( Device device );
        Task<PageList<Device>> SearchDevices( List<int> deviceIds = null, int? deviceGroupId = null, bool? excludePostDevice = null, bool? isOnLine = null, int? ListType = null, string keywords = null, bool? orderSms = null, int pageIndex = 0, int pageSize = int.MaxValue, DateTime? startTime = null, DateTime? endTime = null );
        Task<Device> GetDeviceById( int deviceId );
        Task<List<Device>> GetDeviceListByIds( int[] deviceIds );
        Task<Device> GetDeviceByDeviceUuid( string uuid );
        Task<Device> InsertGuestDevice( string deviceUuid );
        Task InsertDevice( Device device );
        Task UpdateDevice( Device device );
        Task SetListType( int[] deviceIds, int listType );

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
        Task<PageList<DeviceSms>> SearchDeviceSms( int? deviceId = 0, string keywords = null, bool? sent = null, DateTime? startTime = null, DateTime? endTime = null, int pageIndex = 0, int pageSize = 10000, int filterSensitiveWord = 0, string sensitiveWord = null );

        Task DeleteDeviceSms( DeviceSms deviceSms );
        Task<DeviceSms> GetDeviceSmsById( int id );
        Task<List<DeviceSms>> GetDeviceSmsByIds( int[] deviceSmsIds );
        Task InsertDeviceSms( DeviceSms deviceSms );
        Task InsertDeviceSmsRange( List<DeviceSms> deviceSms );
        Task UpdateDeviceSms( DeviceSms deviceSms );
        Task<DeviceDto> GetDeviceById_Pref( int id );
        Task<DeviceDto> GetDeviceByUuid_Pref( string uuid );
    }
}