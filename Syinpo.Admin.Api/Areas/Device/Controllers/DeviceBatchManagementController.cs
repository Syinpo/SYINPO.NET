using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.Jobs;
using Syinpo.Core;
using Syinpo.Core.Data;
using Syinpo.Core.Domain.Poco;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Core.Hangfire;
using Syinpo.Core.Helpers;
using Syinpo.Model.Dto.Devices;
using Syinpo.Model.Request.Devices;
using Syinpo.Unity.AspNetCore.Attributes;
using Syinpo.Unity.AspNetCore.Controllers;
using Syinpo.Unity.AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Syinpo.Core.Linq;
using Syinpo.Model.Core.Notifications;
using Syinpo.Model.Dto.Notifications;
using Microsoft.Extensions.Configuration;
using Syinpo.BusinessLogic.Safety;
using Syinpo.Core.EasyLicense.License.Validator;
using Syinpo.Model.Dto.Users;
using Syinpo.Model.ViewResult.Devices;
using System.Collections.Concurrent;
using System.Diagnostics;
using Dapper;
using DotNetCore.CAP;
using Syinpo.BusinessLogic.Content;
using Syinpo.Model.Request;
using Syinpo.Model;
using Microsoft.Extensions.Caching.Memory;
using Syinpo.Core.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Syinpo.Core.Monitor;
using MediatR;
using Syinpo.Unity;
using Syinpo.Unity.AspNetCore.Filters;
using Microsoft.Data.SqlClient;
using OpenTelemetry.Trace;
using OpenTelemetry.Trace.Configuration;
using Z.EntityFramework.Extensions;
using Syinpo.Core.Caches;
using Syinpo.Core.Cap;
using Syinpo.Model.Core.Content;
using Syinpo.Unity.Firewall;
using Syinpo.Unity.Firewall.Policy;
using Syinpo.Unity.Firewall.Rules;
using Microsoft.Extensions.Options;

namespace Syinpo.Admin.Api.Areas.Device.Controllers {
    /// <summary>
    /// 设备批次管理
    /// </summary>
    [SwaggerTag( "设备批次上报" )]
    public class DeviceBatchManagementController : BaseApiController {
        #region 字段
        private readonly IEventQueueService _eventQueueService;
        private readonly CapBusOptions _capBusOptions;
        private readonly IDeviceService _deviceService;
        private readonly ICurrent _current;
        private readonly IDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DeviceBatchManagementController> _logger;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IMediator _mediator;
        private readonly ICapPublisher _capBus;
        private readonly TracerFactory _tracerFactory;
        private readonly Tracer _tracer;
        private readonly DbContextFactory _dbContextFactory;
        private readonly ICache _cache;

        #endregion

        #region 常量

        private const string CurrentDeviceId = "currentDeviceId";
        private const string TraceId = "traceId";
        private const string CapMsgName = "cap-msg-name";
        private const string CustomVersion = "customVersion";
        private const string ItemId = "ItemId";

        #endregion


        #region 构造函数

        public DeviceBatchManagementController( IEventQueueService eventQueueService,
            IOptions<CapBusOptions> capBusOptions,
            IDeviceService deviceService,
            ICurrent current,
            IDbContext dbContext,
            IConfiguration configuration,
            IMemoryCache memoryCache,
            IBackgroundJobManager backgroundJobManager,
            ILogger<DeviceBatchManagementController> logger,
            IMediator mediator,
            ICapPublisher capPublisher,
            TracerFactory tracerFactor,
            Tracer tracer,
            DbContextFactory dbContextFactory,
            ICache cache ) {
            _eventQueueService = eventQueueService;
            _capBusOptions = capBusOptions?.Value;
            _deviceService = deviceService;
            _current = current;
            _dbContext = dbContext;
            _configuration = configuration;
            _logger = logger;
            _backgroundJobManager = backgroundJobManager;
            _mediator = mediator;
            _capBus = capPublisher;
            _tracerFactory = tracerFactor;
            _tracer = tracer;
            _dbContextFactory = dbContextFactory;
            _cache = cache;
        }
        #endregion

        #region 公共方法

        private EventQueue PreEventQueue( string version, string route, object data, Dictionary<string, string> heads ) {
            if( !heads.ContainsKey( CurrentDeviceId ) )
                throw new Exception( "currentDeviceId不可为空" );

            string context = JsonHelper.ToJson( data );
            string head = JsonHelper.ToJson( heads );

            return new EventQueue {
                Version = version,
                RouteName = route,
                Heads = head,
                Content = context,
                Retry = 0,
                OutTime = null,
                Note = null,
                CreateTime = DateTime.Now
            };
        }


        #endregion

        /// <summary>
        /// 批量上报接口
        /// </summary>
        /// <param name="model">DeviceBatchUploadModel对象</param>
        /// <returns>返回Response</returns>
        /// <remarks>
        /// 移动端每15秒上传一次批次请求，每批次最大100条。
        /// 服务端返回状态为0，表示数据全部验证成功，移动端不需要考虑重试。
        /// 服务端返回状态码为9999，表示数据全部处理失败，移动端需要考虑下一次重试时间。
        /// 服务端返回状态码为1000，表示部分成功，并返回了错误项，移动端需要考虑出错项的下一次重试时间。
        /// 服务端返回状态码为500，表示服务端代码异常，移动端可以考虑重新上报和下次重试时间。
        /// </remarks>
        [HttpPost]
        [Route( "/api/device/batch/create" )]
        [SwaggerResponse( 0, "成功", typeof( Response<string> ) )]
        [SwaggerResponse( -1, "失败", typeof( Response<List<DeviceBatchCreateResult>> ) )]
        [DeviceAuthorize]
        [FirewallValidate( FirewallPolicyEnum.DeviceInTimeMaxRequestRule_s10_c1, FirewallPolicyEnum.DisableRequestRule )]
        public async Task<IActionResult> CreateBatch( DeviceBatchUploadModel model ) {
            if( _capBusOptions == null )
                return Error( propertyKey: "cap", errorMessage: "服务端配置异常" );

            if( model == null || !model.BatchItems.Any() )
                return Error( propertyKey: "Model", errorMessage: "参数不能为空" );

            var device = _current.Device;
            if( device == null )
                return Error( propertyKey: "Device", errorMessage: "设备不存在" );

            var errorItems = new List<DeviceBatchCreateResult>();
            var bathItems = new List<EventQueue>();

            foreach( var item in model.BatchItems ) {
                try {
                    if( string.IsNullOrEmpty( item.Id ) )
                        continue;

                    if( string.IsNullOrEmpty( item.RequestRoute ) )
                        continue;

                    var eventItem = PreEventQueue(
                        _capBusOptions.VersionName,
                        item.RequestRoute,
                        item.Content,
                        new Dictionary<string, string>
                        {
                            {
                                CurrentDeviceId, device.Id.ToString()
                            },
                            {
                                TraceId, _tracer.CurrentSpan.Context.TraceId.ToHexString()
                            },
                            {
                                ItemId, item.Id.ToString()
                            },
                        } );

                    bathItems.Add( eventItem );
                }
                catch( Exception ex ) {
                    errorItems.Add( new DeviceBatchCreateResult { Id = item.Id, ErrorDetails = ex.ToString() } );
                }
            }


            if( bathItems.Any() ) {
                await _eventQueueService.InsertRangeEventQueue( bathItems );
            }

            if( errorItems.Any() && errorItems.Count == model.BatchItems.Count )
                return new JsonResult( new ErrorObject2 { Code = 9999, Message = "数据全部错误", Data = errorItems } );

            if( errorItems.Any() )
                return new JsonResult( new ErrorObject2 { Code = 1000, Message = "数据部分错误", Data = errorItems } );

            return Success( CommonHelper.GetServerName() );
        }

    }
}