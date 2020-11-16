using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EntitySignal.Client.Enums;
using Syinpo.Core.Helpers;
using Syinpo.Core.Reflection;
using Syinpo.Core.SignalBus.Model;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Syinpo.Core.SignalBus.Client {
    public class SignalrClient {
        private readonly ILogger<SignalrClient> _logger;
        private readonly SysOptions _options;
        private readonly IMediator _mediator;

        private List<string> _subscriptions = new List<string>();
        private HubConnection _hub;
        private string _connectionId;
        public SignalrStatus Status { get; set; } = SignalrStatus.Disconnected;

        public SignalrClient( ILogger<SignalrClient> logger, IOptions<SysOptions> options, IMediator mediator ) {
            _mediator = mediator;
            _logger = logger;
            _options = options?.Value;

            _subscriptions = _options?.SignalrBus?.DefaultTopics;

            _hub = new HubConnectionBuilder()
                .WithUrl( _options?.SignalrBus?.HubUrl )
                //.WithAutomaticReconnect()
                .Build();

            _hub.Closed += OnClose;

            _hub.On<SignalrMessage>( "Sync", async data =>
            {
                var type = ReflectionUtils.CreateInstanceFromString( data.Type ).GetType();
                var obj = JObject.Parse( data.JsonData) .ToObject( type );
                await _mediator.Publish( obj );

                //if( data.SmgType == SignalrMessageTypeEnum.Push ) {

                //}
            } );
        }

        private Task OnClose( Exception arg ) {
            Status = SignalrStatus.Disconnected;
            return Reconnect();
        }

        public async Task Connect() {
            if( Status == SignalrStatus.Connected ) {
                return;
            }

            if( Status == SignalrStatus.Connecting || Status == SignalrStatus.WaitingForConnectionId ) {
                return;
            }


            if( Status == SignalrStatus.Disconnected ) {
                Status = SignalrStatus.Connecting;

                _hub.On<string>( "ConnectionIdChanged", connectionId =>
                {
                    _hub.Remove( "ConnectionIdChanged" );
                    Status = SignalrStatus.Connected;
                    _connectionId = connectionId;
                } );

                try {
                    await _hub.StartAsync();

                    if( Status == SignalrStatus.Connecting ) {
                        Status = SignalrStatus.WaitingForConnectionId;
                    }

                    await Task.Delay( _options.SignalrBus.MaxWaitForConnectionId );

                    if( _subscriptions.Any() ) {
                        await AddTopic( _subscriptions );
                    }
                }
                catch( Exception ex ) {
                    Status = SignalrStatus.Disconnected;
                }
            }
        }

        private async Task Reconnect() {
            await Connect();

            if( Status == SignalrStatus.Connected ) {
                //if( _subscriptions.Any() ) {
                //    await AddTopic( _subscriptions );
                //}
            }
            else {
                var random = new Random();
                var reconnectTime = _options.SignalrBus.ReconnectMinTime + ( random.NextDouble() * _options.SignalrBus.ReconnectVariance );
                await Task.Delay( (int)reconnectTime );
                await Reconnect();
            }
        }

        public async Task Dispose() {
            await _hub.DisposeAsync();
        }


        public async Task AddTopic( List<string> topics ) {
            try {
                if( topics.Any() ) {
                    await _hub.InvokeAsync( "AddTopic", topics, CommonHelper.GetServerName() );

                    foreach( var topic in topics ) {
                        if( !_subscriptions.Contains( topic ) ) {
                            _subscriptions.Add( topic );
                        }
                    }
                }
            }
            catch( Exception ex ) {
                _logger.LogError( ex, "SignalrClient AddTopic error:" + ex.Message );
            }
        }

        public async Task Push( string topic, SignalrMessage data ) {
            try {
                if( topic == null || data == null ) {
                    return;
                }

                var response = await _hub.InvokeAsync<SignalrResponse>( "receivePush", topic, data );
                if( !response.Success ) {

                }
            }
            catch( Exception ex )
            {
                _logger.LogError(ex, "SignalrClient push error:" + ex.Message);
            }
        }


        public async Task<T> ServiceCallByFunc<T>( string serverName, string methodName, params object[] parameters ) {
            try {
                 var obj =  await _hub.InvokeAsync<object>( "ServiceCallByFunc", serverName, methodName, parameters );

                 var jsonPars = System.Text.Json.JsonSerializer.Serialize( obj );
                 var result = JsonHelper.ToObject<T>( jsonPars );


                 return result;

            }
            catch( Exception ex ) {
                _logger.LogError( ex, "SignalrClient ServiceCallByFunc error:" + ex.Message );

                return default(T);
            }
        }

        public async Task ServiceCallByAction( string serverName, string methodName, params object[] parameters ) {
            try {
                await _hub.InvokeAsync( "ServiceCallByAction", serverName, methodName, parameters );
            }
            catch( Exception ex ) {
                _logger.LogError( ex, "SignalrClient ServiceCallByAction error:" + ex.Message );
            }
        }
    }
}
