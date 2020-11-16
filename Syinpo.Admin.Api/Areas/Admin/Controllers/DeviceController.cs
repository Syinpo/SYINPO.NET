using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.Notifications.SendHandlers;
using Syinpo.BusinessLogic.SignalR.Online;
using Syinpo.BusinessLogic.Users;
using Syinpo.Core;
using Syinpo.Core.Data;
using Syinpo.Core.Domain.Poco;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Core.Helpers;
using Syinpo.Core.Mapper;
using Syinpo.Model;
using Syinpo.Model.Dto.Devices;
using Syinpo.Model.Request.Devices;
using Syinpo.Unity.AspNetCore.Attributes;
using Syinpo.Unity.AspNetCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Syinpo.Admin.Api.Areas.Admin.Controllers {
    [SwaggerTag( "设备管理" )]
    public class DeviceController : BaseAdminApiController {
        #region 字段

        private readonly ICurrent _current;
        private readonly IDeviceService _deviceService;
        private readonly IPushService _pushService;
        private readonly IOnlineManager _onlineManager;
        private readonly IDbContext _dbContext;
        private readonly DbContextFactory _DbContextFactory;
        private readonly ILogger<DeviceController> _logger;
        #endregion

        #region 构造函数

        public DeviceController( IDeviceService deviceService, ICurrent current, IPushService pushService, IOnlineManager onlineManager, DbContextFactory DbContextFactory, IDbContext dbContext, ILogger<DeviceController> logger ) {
            _current = current;
            _deviceService = deviceService;
            _pushService = pushService;
            _onlineManager = onlineManager;
            _dbContext = dbContext;
            _DbContextFactory = DbContextFactory;
            _logger = logger;
        }

        #endregion

        #region 公共方法

        private DeviceDto PreDeviceModel( Core.Domain.Poco.Device device ) {
            if( device == null )
                throw new Exception( "device is null" );

            return device.MapTo<DeviceDto>();
        }


        #endregion

        #region 方法

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="request">入参</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        [Route( "/api/admin/device/list" )]
        [SwaggerResponse( 0, "成功", typeof( Response<PageResult<DeviceDto>> ) )]
        [SwaggerResponse( -1, "失败", typeof( ErrorObject ) )]
        public async Task<IActionResult> GetDeviceList( DeviceListForAdminParametersModel request ) {
            if( request == null )
                return Error( propertyKey: "request", errorMessage: "request为空" );

            int page = request.Page;
            int pageSize = request.PageSize;

            if( page < 0 )
                page = 1;
            if( pageSize < 0 )
                pageSize = 20;
            if( !request.PartnerId.HasValue )
                request.PartnerId = 0;

            var devices = ( await _deviceService.SearchDevices( isOnLine: request.IsOnline, ListType: request.ListType, keywords: request.KeyWords, pageIndex: page - 1, pageSize: pageSize ) );

            var result = devices.Select( PreDeviceModel ).ToList();

            return Success( new PageResult<DeviceDto> {
                Results = result,
                Page = page,
                PageSize = pageSize,
                TotalCount = devices.TotalCount,
                TotalPages = devices.TotalPages,
            } );
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model">设备</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        [Route( "/api/admin/device/add" )]
        [SwaggerResponse( 0, "成功", typeof( Response<DeviceDto> ) )]
        [SwaggerResponse( -1, "失败", typeof( ErrorObject ) )]
        public async Task<IActionResult> Add( DeviceForAdminEditDto model ) {
            return await _DbContextFactory.UseTransaction( async () => {
                if( model == null ) {
                    return Error( propertyKey: "model", errorMessage: "model为空" );
                }

                if( model.Id < 0 )
                    model.Id = 0;

                if( string.IsNullOrWhiteSpace( model.DeviceUuid ) ) {
                    return Error( propertyKey: "DeviceUuid", errorMessage: "DeviceUuid为空" );
                }
                model.DeviceUuid = model.DeviceUuid.Trim();


                var deviceExeist = await _deviceService.GetDeviceByDeviceUuid( model.DeviceUuid );
                if( deviceExeist != null )
                    return Error( propertyKey: "deviceExeist", errorMessage: "设备存在" );


                var device = model.MapTo<Core.Domain.Poco.Device>();
                device.TrackingId = CommonHelper.NewSequentialGuid().ToString();
                device.Deleted = false;
                device.CreateTime = DateTime.Now;
                device.UpdateTime = DateTime.Now;

                await _deviceService.InsertDevice( device );
                return Success( device.MapTo<DeviceDto>() );

            } );
        }



        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model">设备</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        [Route( "/api/admin/device/update" )]
        [SwaggerResponse( 0, "成功", typeof( Response<DeviceDto> ) )]
        [SwaggerResponse( -1, "失败", typeof( ErrorObject ) )]
        public async Task<IActionResult> Update( DeviceForAdminEditDto model ) {
            if( model == null ) {
                return Error( propertyKey: "model", errorMessage: "model为空" );
            }

            if( model.Id == 0 )
                return Error( propertyKey: "id", errorMessage: "deviceId为空" );

            var device = await _deviceService.GetDeviceById( model.Id );
            if( device == null ) {
                return Error( propertyKey: "device", errorMessage: "设备为空或不在线" );
            }


            device.AssistantVersion = model.AssistantVersion;
            device.WeiXinVersion = model.WeiXinVersion;
            device.Model = model.Model;
            device.Approved = model.Approved;
            device.Brand = model.Brand;
            device.Latitude = model.Latitude;
            device.Longitude = model.Longitude;
            device.Mobile = model.Mobile;
            device.Os = model.Os;
            device.OsVersion = model.OsVersion;
            device.Memo = model.Memo;
            device.UpdateTime = DateTime.Now;
            await _deviceService.UpdateDevice( device );

            return Success( device.MapTo<DeviceDto>() );

        }



        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">入参</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        [Route( "/api/admin/device/delete/{id}" )]
        [SwaggerResponse( 0, "成功", typeof( Response<bool> ) )]
        [SwaggerResponse( -1, "失败", typeof( ErrorObject ) )]
        public async Task<IActionResult> Delete( int id ) {
            if( id < 0 )
                return Error( propertyKey: "id", errorMessage: "id不能为空" );

            var device = await _deviceService.GetDeviceById( id );
            if( device == null ) {
                return Error( propertyKey: "device", errorMessage: "设备为空或不在线" );
            }

            await _deviceService.DeleteDevice( device );
            return Success( true );

        }


        #endregion

    }
}