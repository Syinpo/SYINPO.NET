using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Syinpo.Core.Domain.MonitorPoco;
using Syinpo.Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Syinpo.Core.Monitor {
    public class MonitorTimeDataManager : IMonitorTimeDataManager {
        private readonly IOptions<MonitorOptions> _settings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MonitorService> _logger;

        /// <summary>
        /// 在线客户端
        /// </summary>
        protected ConcurrentDictionary<string, List<Tuple<string, object>>> Clients {
            get;
        }

        /// <summary>
        /// 并发锁
        /// </summary>
        protected readonly object SyncObj = new object();

        public MonitorTimeDataManager( IHttpClientFactory httpClientFactory, IOptions<MonitorOptions> settings, ILogger<MonitorService> logger ) {
            Clients = new ConcurrentDictionary<string, List<Tuple<string, object>>>();
            _settings = settings;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// 添加分时数据
        /// </summary>
        /// <param name="timeGroup"></param>
        /// <param name="context"></param>
        /// <param name="data"></param>
        public void AddData( string timeGroup, string context, object data ) {
            lock( SyncObj ) {
                var item = Tuple.Create( context, data );
                if( Clients.ContainsKey( timeGroup ) )
                    Clients[ timeGroup ].Add( item );
                else {
                    Clients.TryAdd( timeGroup, new List<Tuple<string, object>> { item } );
                }
            }
        }


        /// <summary>
        /// 添加分时数据
        /// </summary>
        /// <param name="timeGroup"></param>
        /// <param name="data"></param>
        public void AddRang( string timeGroup, List<Tuple<string, object>> data ) {
            lock( SyncObj ) {
                if( Clients.ContainsKey( timeGroup ) )
                    Clients[ timeGroup ].AddRange( data );
                else {
                    Clients.TryAdd( timeGroup, data );
                }
            }
        }

        public async Task Flush() {
            var now = DateTimeHelper.GetTimeGroup( DateTime.Now );
            var keys = Clients.Keys.Where( w => w != now ).ToList();
            foreach( var key in keys ) {
                Clients.TryGetValue( key, out List<Tuple<string, object>> logs );

                if( logs != null ) {
                   // await SendBatch( logs, key );
                }

                Clients.TryRemove( key, out var client );
            }
        }

        public void ServerFlush() {
            lock( SyncObj ) {
                var now = DateTimeHelper.GetTimeGroup( DateTime.Now );
                var keys = Clients.Keys.Where( w => w != now ).ToList();
                foreach( var key in keys ) {
                    Clients.TryGetValue( key, out List<Tuple<string, object>> logs );

                    if( logs != null ) {
                        TrafficStatist trafficStatist = new TrafficStatist();
                        trafficStatist.DateTime = Convert.ToInt64( key );
                        trafficStatist.TotalHttpReq = logs.Count( w => w.Item1 == MonitorContextKeys.http );
                        trafficStatist.TotalSignalrReq = logs.Count( w => w.Item1 == MonitorContextKeys.signalr );
                        trafficStatist.TotalReq = trafficStatist.TotalHttpReq + trafficStatist.TotalSignalrReq;
                        trafficStatist.ExceptionReq = logs.Count( w => w.Item1 == MonitorContextKeys.fault );

                        //
                        trafficStatist.TotalSignalrConnectReq = logs.Count( w => w.Item1 == MonitorContextKeys.signalr_connect );
                        trafficStatist.TotalSignalrDisconnectReq = logs.Count( w => w.Item1 == MonitorContextKeys.signalr_disconnect );

                        var onlines = logs.Where( w => w.Item1 == MonitorContextKeys.online_device ).ToList();
                        if( onlines.Any() ) {
                            trafficStatist.TotalOnlineDevice = (long)onlines.Average( a => Convert.ToInt64( a.Item2 ) );
                        }

                        // server
                        var cpu_server = logs.Where( w => w.Item1 == MonitorContextKeys.cpu_server ).ToList();
                        if( cpu_server.Any() ) {
                            trafficStatist.ServerCpuAvgRate = cpu_server.Average( a => Convert.ToDecimal( a.Item2 ) );
                            trafficStatist.ServerCpuTopRate = cpu_server.Max( a => Convert.ToDecimal( a.Item2 ) );
                        }

                        var ram_server = logs.Where( w => w.Item1 == MonitorContextKeys.ram_server ).ToList();
                        if( ram_server.Any() ) {
                            trafficStatist.ServerRamAvgRate = ram_server.Average( a => Convert.ToDecimal( a.Item2 ) );
                            trafficStatist.ServerRamTopRate = ram_server.Max( a => Convert.ToDecimal( a.Item2 ) );
                        }

                        // app
                        var cpu_app = logs.Where( w => w.Item1 == MonitorContextKeys.cpu_app ).ToList();
                        if( cpu_app.Any() ) {
                            trafficStatist.AppliationCpuAvgRate = cpu_app.Average( a => Convert.ToDecimal( a.Item2 ) );
                            trafficStatist.AppliationCpuTopRate = cpu_app.Max( a => Convert.ToDecimal( a.Item2 ) );
                        }

                        var ram_app_size = logs.Where( w => w.Item1 == MonitorContextKeys.ram_app_size ).ToList();
                        if( ram_app_size.Any() ) {
                            trafficStatist.AppliationRamAvgSize = ram_app_size.Average( a => Convert.ToDecimal( a.Item2 ) );
                        }

                        //trafficStatist.AppliationRamAvgRate = logs
                        //    .Where( w => w.Item1 == MonitorContextKeys.ram_app )
                        //    .Average( a => (decimal)a.Item2 );
                        //trafficStatist.AppliationRamTopRate = logs
                        //    .Where( w => w.Item1 == MonitorContextKeys.ram_app )
                        //    .Max( a => (decimal)a.Item2 );


                        trafficStatist.CreateTime = DateTime.Now;

                        var path = CommonHelper.MapPath( _settings.Value.TimeDataStorePath + $"{key}.log" );
                        JsonHelper.ToPack( path, trafficStatist );
                    }

                    Clients.TryRemove( key, out var client );
                }
            }
        }


        public async Task SendBatch( List<Tuple<string, object>> items, string timeGroup ) {
            var client = _httpClientFactory.CreateClient( "monitor" );

            var message = new HttpRequestMessage();

            var content = JsonHelper.ToJson( items );
            message.Content = new StringContent( content, Encoding.UTF8, "application/json" );
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri( $"{_settings.Value.ServerAddress}api/batch/timedata/{timeGroup}" );

            var resp = await client.SendAsync( message );
            if( !resp.IsSuccessStatusCode )
                _logger.LogWarning( "MonitorTimeDataManager SendBatch erros:{StatusCode}", resp.StatusCode );
        }

    }
}
