using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.Jobs;
using Syinpo.BusinessLogic.Notifications;
using Syinpo.BusinessLogic.Safety;
using Syinpo.Core;
using Syinpo.Core.Caches;
using Syinpo.Core.Data;
using Syinpo.Core.Domain.Poco;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Core.Extensions;
using Syinpo.Core.Hangfire;
using Syinpo.Core.Helpers;
using Syinpo.Core.Mapper;
using Syinpo.Core.Monitor;
using Syinpo.Model;
using Syinpo.Model.Core.Notifications;
using Syinpo.Model.Dto.Devices;
using Syinpo.Model.Dto.Notifications;
using Syinpo.Model.Request.Devices;
using Syinpo.Model.ViewResult.Devices;
using Syinpo.Unity.AspNetCore.Attributes;
using Syinpo.Unity.AspNetCore.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OpenTelemetry.Trace;
using OpenTelemetry.Trace.Configuration;
using Swashbuckle.AspNetCore.Annotations;

namespace Syinpo.Cap.Api.Areas.Device.Controllers {
    /// <summary>
    /// 设备批次管理
    /// </summary>
    [SwaggerTag( "设备批次上报" )]
    public class DeviceBatchManagementController : BaseApiController {
        #region 字段
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

        #endregion


        #region 构造函数

        public DeviceBatchManagementController( IDeviceService deviceService,
            ICurrent current, IDbContext dbContext,
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

        private async Task<TelemetrySpan> PreBefore( CapHeader header ) {
            if( !header.ContainsKey( CurrentDeviceId ) )
                throw new Exception( "currentDeviceId不可为空" );

            _dbContextFactory.IsTransaction = true;

            var deviceId = Convert.ToInt32( header[ CurrentDeviceId ] );
            var deviceDto = await _deviceService.GetDeviceById_Pref( deviceId );
            _current.Device = deviceDto;

            var tracer = _tracerFactory.GetTracer( header[ CapMsgName ] );
            var traceId = header.ContainsKey( TraceId ) ? header[ TraceId ] : CommonHelper.NewSequentialGuid().ToString();
            var spanId = tracer.CurrentSpan.Context.SpanId;
            var spanContext = new SpanContext(
                ActivityTraceId.CreateFromString( traceId ),
                spanId, //  ActivitySpanId.CreateFromString(""),
                ActivityTraceFlags.None );

            return tracer.StartSpan( header[ CapMsgName ], spanContext, SpanKind.Server );
        }


        private async Task<Tuple<DeviceDto, T>> PreItem<T>( EventQueue eventQueue ) {
            var header = JsonHelper.ToObject<Dictionary<string, string>>( eventQueue.Heads );
            if( header == null || !header.ContainsKey( CurrentDeviceId ) )
                return null;

            var deviceId = Convert.ToInt32( header[ CurrentDeviceId ] );
            var deviceDto = await _deviceService.GetDeviceById_Pref( deviceId );
            if( deviceDto == null )
                return null;

            _current.Device = deviceDto;

            var model = JsonHelper.ToObject<T>( eventQueue.Content );

            return Tuple.Create( deviceDto, model );
        }

        #endregion
        
        #region DeviceSms

        /// <summary>
        /// 上传短信
        /// </summary>
        /// <param name="items">入参</param>
        /// <returns>返回Response</returns>
        /// <remarks>
        /// 上传短信。
        /// </remarks>
        [NonAction]
        [CapSubscribe( "/batch/api/device/sms/create", Group = "cap.queue.syinpo.cap.api" )]
        public async Task<IActionResult> CreateDeviceSms( List<EventQueue> items, [FromCap] CapHeader header ) {
            var inserts = new List<DeviceSms>();

            foreach( var item in items ) {
                var data = await PreItem<DeviceSmsForDeviceDto>( item );
                if( data == null )
                    continue;

                var device = data.Item1;
                var dto = data.Item2;

                var sms = dto.MapTo<DeviceSms>();

                if( string.IsNullOrEmpty( sms.FromPhone ) )
                    sms.FromPhone = device.Mobile;
                if( string.IsNullOrEmpty( sms.FromPhone ) )
                    sms.FromPhone = " ";

                if( string.IsNullOrEmpty( sms.ToPhone ) )
                    sms.ToPhone = device.Mobile;
                if( string.IsNullOrEmpty( sms.ToPhone ) )
                    sms.ToPhone = " ";


                sms.CreateTime = DateTime.Now;
                sms.DeviceId = device.Id;
                sms.FromPhone = sms.FromPhone?.Replace( "+86", "" ).Trim();
                sms.ToPhone = sms.ToPhone?.Replace( "+86", "" ).Trim();
                inserts.Add( sms );
            }

            if( inserts.Any() )
                await _deviceService.InsertDeviceSmsRange( inserts );

            return Success();
        }

        #endregion

    }
}