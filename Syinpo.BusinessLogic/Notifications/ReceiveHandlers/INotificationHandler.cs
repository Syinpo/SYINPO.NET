using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Syinpo.BusinessLogic.Notifications.ReceiveHandlers {
    public interface INotificationHandler {

        bool IsMatch( string notifyType );

        Task<object> Execute( JObject body );

    }
}
