using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Syinpo.Core.Caches;
using Syinpo.Core.Helpers;
using Syinpo.Core.Reflection;
using Syinpo.Core.SignalBus.Model;
using Microsoft.AspNetCore.SignalR;

namespace Syinpo.Core.SignalBus.Server {

    public class AgentHub : Hub<ISignalrHubClient> {

        #region 字段

        private readonly IMd5Cache _md5Cache;

        #endregion


        #region 构造函数

        public AgentHub() {
            // _md5Cache = IoC.Resolve<IMd5Cache>( "DeviceHub" );
        }

        #endregion


        #region Connect
        public override async Task OnConnectedAsync() {
            await base.OnConnectedAsync();
            await Clients.Caller.ConnectionIdChanged( Context.ConnectionId );
        }

        public override async Task OnDisconnectedAsync( Exception exception ) {
            await SignalrDataStore.RemoveConnection( Context.ConnectionId );
            await base.OnDisconnectedAsync( exception );
        }
        #endregion

        #region Topic

        public void AddTopic( List<string> topics, string serverName ) {
            foreach( var topic in topics ) {
                SignalrDataStore.AddUser( new Core.SignalBus.Model.UserSubscription {
                    ConnectionId = Context.ConnectionId,
                    Topic = topic,
                    TopicTime = DateTime.Now,
                    FromServerName = serverName,
                    IpAddress = CommonHelper.GetClientIpAddress()
                } );
            }
        }

        public void DelTopic( string topic ) {
            SignalrDataStore.RemoveTopic( Context.ConnectionId, topic );
        }


        /// <summary>
        /// 客户端调用
        /// </summary>
        /// <returns></returns>
        public async Task<SignalrResponse> receivePush( string topic, SignalrMessage data ) {
            try {
                if( topic == null || data == null ) {
                    return Error( "topic or message is null" );
                }


                var pendingTasks = new List<Task>();

                SubscriptionsByTopic subscriptionsByTopic;
                SignalrDataStore.SubscriptionsByTopic.TryGetValue( topic, out subscriptionsByTopic );

                if( subscriptionsByTopic != null ) {
                    var subscribedUsers = subscriptionsByTopic.SubscriptionsByUser;

                    foreach( var subscribedUser in subscribedUsers ) {
                        if( subscribedUser.Key == null ) {
                            continue;
                        }

                        var newTask = Clients.Client( subscribedUser.Key ).Sync( data );
                        pendingTasks.Add( newTask );
                    }
                }

                await Task.WhenAll( pendingTasks );

                return Success();
            }
            catch( Exception ex ) {
                return Error( ex.Message );
            }
        }

        #endregion

        #region Service Call

        public object ServiceCallByFunc( string serverName, string methodName, params object[] parameters ) {
            var type = ReflectionUtils.GetTypeFromName( serverName );
            var service = IoC.Resolve( type );

            var jsonPars = System.Text.Json.JsonSerializer.Serialize( parameters );
            var newObjs = JsonHelper.ToObject<object[]>( jsonPars );

            return ReflectionUtils.CallMethod( service, methodName, newObjs );
        }

        public void ServiceCallByAction( string serverName, string methodName, params object[] parameters ) {
            var type = ReflectionUtils.GetTypeFromName( serverName );
            var service = IoC.Resolve( type );

            var jsonPars = System.Text.Json.JsonSerializer.Serialize( parameters );
            var newObjs = JsonHelper.ToObject<object[]>( jsonPars );

            ReflectionUtils.CallMethod( service, methodName, newObjs );
        }

        #endregion

        #region 公共方法

        protected SignalrResponse Error( string errorMessage = "error", object errorData = null ) {
            var errorsRootObject = new SignalrResponse {
                Message = errorMessage,
                Success = false,
                Data = errorData
            };

            return errorsRootObject;
        }

        protected SignalrResponse Success( string message = "ok" ) {
            var errorsRootObject = new SignalrResponse {
                Message = message,
                Success = true
            };

            return errorsRootObject;
        }

        #endregion

    }
}
