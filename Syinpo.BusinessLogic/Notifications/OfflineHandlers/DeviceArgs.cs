using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Syinpo.BusinessLogic.Notifications.OfflineHandlers {
    public class DeviceArgs : INotification {
        public int DeviceId {
            get; set;
        }

        public bool IsOnline {
            get;set;
        }
    }
}
