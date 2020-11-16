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
    public class PushUserProcessHandler : INotificationHandler<UserNotification> {
        #region 字段

        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IRealTimeNotifier _realTimeNotifier;
        private readonly IDeviceService _deviceService;
        private readonly ILogger<PushUserProcessHandler> _logger;

        #endregion

        #region 构造函数

        public PushUserProcessHandler( IBackgroundJobManager backgroundJobManager, IRealTimeNotifier realTimeNotifier, IDeviceService deviceService, ILogger<PushUserProcessHandler> logger ) {
            _backgroundJobManager = backgroundJobManager;
            _realTimeNotifier = realTimeNotifier;
            _deviceService = deviceService;
            _logger = logger;
        }

        #endregion


        public async Task Handle( UserNotification args, CancellationToken cancellationToken ) {
            try {
               // await _realTimeNotifier.SendUserNotificationsAsync( args );
            }
            catch( Exception ex ) {
                _logger.LogWarning( "PushUserProcessHandler error: " + ex.ToString(), ex );
            }
        }
    }
}
