using System.Threading.Tasks;
using Syinpo.Model.Core.Notifications;

namespace Syinpo.BusinessLogic.SignalR.Notifications {
    // internal 不许改成 public
    public interface IRealTimeNotifier
    {
        Task SendDeviceNotificationsAsync( DeviceNotification deviceNotification );
        Task SendDeviceNotificationsAsync( DeviceNotification[] deviNotifications );
    }
}
