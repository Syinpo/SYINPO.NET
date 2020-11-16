using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Syinpo.Core.SignalBus.Model;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Syinpo.Core.SignalBus.Server {
   public class SignalrDataProcess {
        private readonly IHubContext<AgentHub, ISignalrHubClient> _dataHubContext;

        public SignalrDataProcess(IHubContext<AgentHub, ISignalrHubClient> dataHubContext ) {
            _dataHubContext = dataHubContext;
        }



        public async Task Post( string topic, SignalrMessage data ) {
            if( topic == null || data == null ) {
                return;
            }


            var pendingTasks = new List<Task>();

            SubscriptionsByTopic subscriptionsByTopic;
            SignalrDataStore.SubscriptionsByTopic.TryGetValue( topic, out subscriptionsByTopic );

            if( subscriptionsByTopic != null ) {
                var subscribedUsers = subscriptionsByTopic.SubscriptionsByUser;

                foreach( var subscribedUser in subscribedUsers ) {
                    if( subscribedUser.Key == null ) {
                        return;
                    }

                    var newTask = _dataHubContext.Clients.Client( subscribedUser.Key ).Sync( data );
                    pendingTasks.Add( newTask );
                }
            }

            await Task.WhenAll( pendingTasks );
        }
    }
}
