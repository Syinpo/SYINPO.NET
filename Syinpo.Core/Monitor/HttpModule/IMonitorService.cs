using System.Threading.Tasks;
using Syinpo.Core.Monitor.TracerModule;

namespace Syinpo.Core.Monitor
{
    public interface IMonitorService
    {
        void QueueEvent( MonitorEvent ev );
        Task Flush();
    }
}