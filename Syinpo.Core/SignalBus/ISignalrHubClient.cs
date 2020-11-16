using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Syinpo.Core.SignalBus.Model;

namespace Syinpo.Core.SignalBus {
    public interface ISignalrHubClient {
        Task Sync( SignalrMessage message );
        Task ConnectionIdChanged( string newConnectionId );
    }
}
