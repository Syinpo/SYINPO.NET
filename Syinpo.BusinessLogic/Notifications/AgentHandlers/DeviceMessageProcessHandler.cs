using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.SignalR.Notifications;
using Syinpo.Core.Container;
using Syinpo.Core.Hangfire;
using Syinpo.Model.Core.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Syinpo.BusinessLogic.Notifications.AgentHandlers {


    public class DeviceMessageProcessHandler : INotificationHandler<DeviceMessage<NotificationObject>> {
        #region 字段

        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IRealTimeNotifier _realTimeNotifier;
        private readonly IDeviceService _deviceService;
        private readonly ILogger<DeviceMessageProcessHandler> _logger;

        #endregion

        #region 构造函数

        public DeviceMessageProcessHandler( IBackgroundJobManager backgroundJobManager, IRealTimeNotifier realTimeNotifier, IDeviceService deviceService,  ILogger<DeviceMessageProcessHandler> logger) {
            _backgroundJobManager = backgroundJobManager;
            _realTimeNotifier = realTimeNotifier;
            _deviceService = deviceService;
            _logger = logger;
        }

        #endregion



        public async Task Handle( DeviceMessage<NotificationObject> notification, CancellationToken cancellationToken ) {
            string msgTypeStr = "0";
            try
            {
                var body = JObject.FromObject(notification.Data.Body);

            }
            catch( Exception ex ) {
                _logger.LogWarning( ex , $" msgType [{msgTypeStr}]  DeviceMessageProcessHandler error: " + ex.Message );
            }
        }
    }
}
