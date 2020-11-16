using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.Jobs;
using Syinpo.BusinessLogic.SignalR.Notifications;
using Syinpo.Core.Hangfire;
using Syinpo.Core.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Syinpo.BusinessLogic.Notifications.OfflineHandlers {
    public class DeviceHandler : INotificationHandler<DeviceArgs> {
        #region 字段

        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IDeviceService _deviceService;
        private readonly ILogger<DeviceHandler> _logger;

        #endregion

        #region 构造函数

        public DeviceHandler( IBackgroundJobManager backgroundJobManager,  IDeviceService deviceService, ILogger<DeviceHandler> logger ) {
            _backgroundJobManager = backgroundJobManager;
            _deviceService = deviceService;
            _logger = logger;
        }

        #endregion


        public async Task Handle( DeviceArgs args, CancellationToken cancellationToken ) {
            try {
                //await _backgroundJobManager.EnqueueAsync<DeviceOfflineNotificationJob, DeviceJobArgs>(
                //    new DeviceJobArgs( args.DeviceId, args.IsOnline ) );
            }
            catch( Exception ex ) {
                _logger.LogWarning( "DeviceHandler error: " + ex.ToString(), ex );
            }
        }
    }
}
