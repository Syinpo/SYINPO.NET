using System;
using System.Threading;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.SignalR.Notifications;
using Syinpo.Core.Hangfire;
using Syinpo.Model.Core.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Syinpo.BusinessLogic.Notifications.AgentHandlers {
    public class PushDeviceProcessHandler : INotificationHandler<DeviceNotification> {
        #region 字段

        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IRealTimeNotifier _realTimeNotifier;
        private readonly IDeviceService _deviceService;
        private readonly ILogger<PushDeviceProcessHandler> _logger;

        #endregion

        #region 构造函数

        public PushDeviceProcessHandler( IBackgroundJobManager backgroundJobManager, IRealTimeNotifier realTimeNotifier, IDeviceService deviceService,  ILogger<PushDeviceProcessHandler> logger ) {
            _backgroundJobManager = backgroundJobManager;
            _realTimeNotifier = realTimeNotifier;
            _deviceService = deviceService;
            _logger = logger;
        }

        #endregion


        public async Task Handle( DeviceNotification args, CancellationToken cancellationToken ) {
            try
            {
                await _realTimeNotifier.SendDeviceNotificationsAsync(args);
            }
            catch( Exception ex ) {
                _logger.LogWarning( "PushProcessHandler error: " + ex.ToString(), ex );
            }
        }
    }
}
