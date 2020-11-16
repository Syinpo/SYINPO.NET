using System.Threading.Tasks;

namespace Syinpo.Core.Monitor.PackModule {
    public interface IPackStore<T> where T : class {
        void AddQueue( T data );

        Task Flush();

        Task Zip();

        Task UnZip();

        Task SendBatch();
    }
}
