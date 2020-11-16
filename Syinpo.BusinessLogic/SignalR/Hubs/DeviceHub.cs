using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Syinpo.BusinessLogic.Caches;
using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.Jobs;
using Syinpo.BusinessLogic.Notifications.OfflineHandlers;
using Syinpo.BusinessLogic.Notifications.ReceiveHandlers;
using Syinpo.BusinessLogic.Notifications.SendHandlers;
using Syinpo.BusinessLogic.Safety;
using Syinpo.BusinessLogic.SignalR.Online;
using Syinpo.Core;
using Syinpo.Core.Caches;
using Syinpo.Core.Container;
using Syinpo.Core.Domain.Identity;
using Syinpo.Core.Domain.Poco;
using Syinpo.Core.Hangfire;
using Syinpo.Core.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Syinpo.Core.Mapper;
using Syinpo.Core.Monitor;
using Syinpo.Core.Monitor.PackModule;
using Syinpo.Core.Reflection;
using Syinpo.Model.Core.Notifications;
using MediatR;
using Newtonsoft.Json.Linq;
using Syinpo.Model.Request;
using Syinpo.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using OpenTelemetry.Trace.Configuration;
using Syinpo.Core.Data;
using Syinpo.Core.SignalBus.Client;
using Org.BouncyCastle.Crypto.Engines;
using Microsoft.EntityFrameworkCore;

namespace Syinpo.BusinessLogic.SignalR.Hubs
{
    /// <summary>
    /// 设备Hub
    /// </summary>
    public class DeviceHub : HubBase
    {

        #region 字段

        private readonly ITokenService _tokenService;
        private readonly IDeviceService _deviceService;
        private readonly ITypeFinder _typeFinder;
        private readonly ITypeResolve _typeResolve;
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPackStore<HttpLog> _logStore;
        private readonly IPackStore<TimeData> _monitorTimeDataManager;
        private readonly Tracer _tracer;
        private readonly TracerFactory _tracerFactory;
        private readonly IOptions<MonitorOptions> _settings;
        private readonly HangfireOptions _hangfireOptions;
        private readonly DbContextFactory _dbContextFactory;
        private readonly ICache _cache;
        private readonly IDbContext _dbContext;
        private readonly IDatabaseHelper _databaseHelper;
        private readonly IDataContainer _dataContainer;
        private readonly SignalrClient _signalrClient;
        private readonly ILogger<DeviceHub> _logger;

        #endregion

        #region 构造函数

        public DeviceHub( ICurrent current, ICache cache, IDbContext dbContext, IDatabaseHelper databaseHelper, IDataContainer dataContainer, SignalrClient signalrClient, IOnlineManager onlineManager, ILogger<DeviceHub> logger, ITokenService tokenService, IDeviceService deviceService, ITypeFinder typeFinder, ITypeResolve typeResolve, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPackStore<HttpLog> logStore, IPackStore<TimeData> monitorTimeDataManager, Tracer tracer, TracerFactory tracerFactor, IOptions<MonitorOptions> settings, DbContextFactory dbContextFactory, IOptions<SysOptions> sysOptions, IOptions<HangfireOptions> hangfireOption = null ) : base( current, onlineManager, logger, sysOptions) {
            _tokenService = tokenService;
            _deviceService = deviceService;
            _typeFinder = typeFinder;
            _typeResolve = typeResolve;
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
            _logStore = logStore;
            _monitorTimeDataManager = monitorTimeDataManager;
            _tracer = tracer;//tracerFactor.GetTracer( "syinpo-signalr-tracer"+CommonHelper.NewSequentialGuid() );
            _tracerFactory = tracerFactor;
            _settings = settings;
            _hangfireOptions = hangfireOption.Value;
            _dbContextFactory = dbContextFactory;
            _cache = cache;
            _dbContext = dbContext;
            _databaseHelper = databaseHelper;
            _dataContainer = dataContainer;
            _signalrClient = signalrClient;
            _logger = logger;
        }

        #endregion

        #region 公共方法
        private void WriteHttpLog(DateTime now, RequestTypeEnum requestType, string traceId, string requestBody, string operation)
        {
            if (_settings.Value.UseMonitor)
            {
                var group = now.ToString("yyyyMMddHHmm");
                long.TryParse(group, out long requestGroup);

                _monitorTimeDataManager.AddQueue(new TimeData(group, MonitorContextKeys.signalr, 1));

                HttpLog _log = new HttpLog()
                {
                    TraceId = traceId,
                    RequestTime = now,
                    RequestType = (int)requestType,
                    RequestGroup = requestGroup,
                    RemoteIpAddress = CommonHelper.GetClientIpAddress(),
                    RemotePort = CommonHelper.GetClientIpPort(),
                    IdentityIsAuthenticated = Current.Device != null,
                    IdentityName = Current.Device?.DeviceUuid,
                    RequestBody = requestBody,
                    RequestContentLength = requestBody == null ? 0 : Encoding.Default.GetByteCount(requestBody),
                    Operation = operation,
                    ServerName = CommonHelper.GetServerName()
                };
                this._logStore.AddQueue(_log);
            }
        }
        #endregion

        #region 方法
        public override async Task OnConnectedAsync() {
            try {
                var now = DateTime.Now;


                if (_settings.Value.UseMonitor)
                {
                    var currentSpan = _tracer.StartRootSpan("syinpo-signalr-connect-tracer", SpanKind.Client);
                    var group = now.ToString("yyyyMMddHHmm");
                    long.TryParse(group, out long requestGroup);

                    _monitorTimeDataManager.AddQueue(new TimeData(group, MonitorContextKeys.signalr_connect, 1));
                    WriteHttpLog(now,
                        RequestTypeEnum.Signalr_Connect,
                        currentSpan.Context.TraceId.ToHexString(),
                        Context?.ConnectionId,
                        "Signalr#Connect");
                    currentSpan.End();
                }

                bool usefulDevice = true;

                if( usefulDevice ) {
                    await base.OnConnectedAsync();

                    if (_hangfireOptions.UseHangfire)
                    {
                        await _mediator.Publish(new DeviceArgs { DeviceId = Current.Device.Id, IsOnline = true });
                    }

                    {
                        var device = await _deviceService.GetDeviceById( Current.Device.Id );
                        device.IsOnline = true;
                        device.UpdateTime = DateTime.Now;
                        await _deviceService.UpdateDevice( device );
                    }
                }
            }
            catch( Exception ex ) {
                Logger.LogWarning( "DeviceHub OnConnected Error:" + ex.ToString(), ex );
            }
        }

        public override async Task OnDisconnectedAsync( Exception exception ) {
            try {
                var now = DateTime.Now;

                if (_settings.Value.UseMonitor)
                {
                    var currentSpan = _tracer.StartRootSpan("syinpo-signalr-disconnect-tracer", SpanKind.Client);
                    var group = now.ToString("yyyyMMddHHmm");
                    long.TryParse(group, out long requestGroup);

                    _monitorTimeDataManager.AddQueue(new TimeData(group, MonitorContextKeys.signalr_disconnect, 1));
                    WriteHttpLog(now,
                        RequestTypeEnum.Signalr_Disconnect,
                        currentSpan.Context.TraceId.ToHexString(),
                        Context?.ConnectionId,
                        "Signalr#Disconnect");

                    currentSpan.End();
                }


                string id = Context?.ConnectionId;

                await base.OnDisconnectedAsync( exception );

                {
                    var device = await _deviceService.GetDeviceById( Current.Device.Id );
                    device.IsOnline = false;
                    device.UpdateTime = DateTime.Now;
                    await _deviceService.UpdateDevice( device );
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning("DeviceHub OnDisconnected Error:" + ex.ToString(), ex);
            }
        }

        /// <summary>
        /// 客户端调用
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<string> receivePush( string message ) {

            var currentSpan = _tracer.StartRootSpan( "syinpo-signalr-tracer", SpanKind.Client );
            string json = string.Empty;
            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Reset();
            _stopwatch.Start();

            var now = DateTime.Now;
            var group = now.ToString("yyyyMMddHHmm");
            long.TryParse(group, out long requestGroup);

            try {
                _databaseHelper.AccorMainDb = true;

                currentSpan.SetAttribute( MonitorKeys.request_type, RequestTypeEnum.Signalr.ToString().ToLowerInvariant() );
                currentSpan.SetAttribute(MonitorKeys.request_type, RequestTypeEnum.Signalr.ToString().ToLowerInvariant());

                try
                {
                    byte[] buffer = Convert.FromBase64String(message);
                    string sourthUrl = Encoding.UTF8.GetString(buffer);
                    //sourthUrl = HttpUtility.UrlDecode( sourthUrl );
                    message = sourthUrl;
                }
                catch
                {
                    // message不是baase64编码，采用原始字符串解析
                }


                if( _settings.Value.UseMonitor ) {
                    _monitorTimeDataManager.AddQueue( new TimeData( group, MonitorContextKeys.signalr, 1 ) );
                    WriteHttpLog( now,
                        RequestTypeEnum.Signalr,
                        currentSpan.Context.TraceId.ToHexString(),
                        message,
                        "Signalr#receivePush");
                }


                if (string.IsNullOrEmpty(message))
                    return Error("消息不能为空").ToJson();

                var type = JsonHelper.ToObject<JsonClassHleper>(message);
                if (type == null || type.Header == null || string.IsNullOrEmpty(type.Header.NotifyType))
                    return Error("消息格式不正确").ToJson();


                var handler = _typeFinder.Find<INotificationHandler>()
                    .Select(x => _typeResolve.Resolve(x) as INotificationHandler)
                    .FirstOrDefault(f => f.IsMatch(type.Header.NotifyType));
                if (handler == null)
                    return Error("handler不存在").ToJson();

                // 入数据容器
                if( handler.GetType().Name.Contains( "DeviceWeiXinMessage" ) ) {
                    await _signalrClient.Push( "DeviceWeiXinMessage", new Core.SignalBus.Model.SignalrMessage {
                        JsonData = new DeviceMessage<NotificationObject> {
                            DeviceId = Current.Device.Id,
                            Data = JsonHelper.ToObject<NotificationObject>( message )
                        }.ToJson(),
                        Type = typeof( DeviceMessage<NotificationObject> ).FullName,
                        SmgType = Core.SignalBus.Model.SignalrMessageTypeEnum.Othor,
                    } );


                    //await _dataContainer.Publish<DeviceMessage<NotificationObject>>(
                    //     string.Format( ContainerKeys.ContainerSignalr_Catetory, type.Header.NotifyType ),
                    //     new DeviceMessage<NotificationObject> {
                    //         DeviceId = Current.Device.Id,
                    //         Data = JsonHelper.ToObject<NotificationObject>( message )
                    //     } );

                    return Success().ToJson();
                }


                //Logger.LogWarning("DeviceHub receivePush before:" + ":" + message);
                var result = await handler.Execute( type.Body );
                json = result.ToJson();
                //Logger.LogWarning("DeviceHub receivePush after:" + ":" + message);

                //currentSpan.SetAttribute( MonitorKeys.response_success, true );
            }
            catch (SysException ex)
            {
                Logger.LogWarning("DeviceHub receivePush error:" + ":" + message + ":" + ex);

                json = Error("DeviceHub receivePush error:" + ex.Message, ex.ErrorData).ToJson();

                currentSpan.SetAttribute(MonitorKeys.request_error, ex.Source);
                currentSpan.SetAttribute(MonitorKeys.request_errordetail, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError("DeviceHub receivePush error:" + ":" + message + ":" + ex);

                json = Error("DeviceHub receivePush error:" + ex.Message).ToJson();

                currentSpan.SetAttribute(MonitorKeys.request_error, ex.Source);
                currentSpan.SetAttribute(MonitorKeys.request_errordetail, ex.Message);
            }
            finally
            {
                if (json.Contains("\"success\": true"))
                {
                    currentSpan.SetAttribute(MonitorKeys.response_success, true);
                }
                else {
                    if( _settings.Value.UseMonitor ) {
                        _monitorTimeDataManager.AddQueue( new TimeData( group, MonitorContextKeys.fault, 1 ) );
                    }
                    currentSpan.SetAttribute(MonitorKeys.response_success, false);
                }

                _stopwatch.Stop();
                currentSpan.SetAttribute(MonitorKeys.response_elapsed, _stopwatch.ElapsedMilliseconds);
                currentSpan.SetAttribute(MonitorKeys.response_statuscode, (int?)null);
                currentSpan.SetAttribute(MonitorKeys.response_body, json);
                currentSpan.SetAttribute(MonitorKeys.response_contentlength, json == null ? 0 : Encoding.Default.GetByteCount(json));
                currentSpan.End();
            }

            return json;
        }

        #endregion

        #region 帮助类

        public class JsonClassHleper
        {
            public NotificationHead Header
            {
                get; set;
            }

            public JObject Body
            {
                get;
                set;
            }

            public class NotificationHead
            {

                public string NotifyType
                {
                    get; set;
                }
            }
        }


        #endregion
    }
}
