using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Monitor;
using Syinpo.Core;
using Syinpo.Core.Data;
using Syinpo.Core.Domain.MonitorPoco;
using Syinpo.Core.Helpers;
using Syinpo.Core.Mapper;
using Syinpo.Core.Monitor;
using Syinpo.Core.Monitor.PackModule;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Syinpo.Monitor.Processor {
    class Program {
        static async Task Main( string[] args ) {
            new BootStart().Start();
            var logger = IoC.Resolve<ILogger<Program>>();
            var options = IoC.Resolve<IOptions<MonitorOptions>>()?.Value;

            // app start
            logger.LogInformation( "Syinpo.Monitor.Processor 启动" );

            while( true ) {
                await UnZip( logger, options.FindRequestLogPath );

                await UnZip( logger, options.FindTimeDataLogPath );

                await Log( logger, options );

                await Time( logger, options );

                await TimeStore( logger, options );

                System.Threading.Thread.Sleep( 1000 * 10 );
            }
        }

        private static async Task UnZip( ILogger<Program> logger, string path ) {
            try {
                logger.LogInformation( "开始批量解压目录文件：" + path );

                var now = DateTime.Now;
                var key = now.ToString( "yyyyMMddHHmm" );

                var path1 = Path.Combine( path, "zip" );
                var path2 = Path.Combine( path, "extract" );

                MonitorZipFileHelper.UnFromDirectory( path1, path2 );

                //var zipFiles = ListFiles( path1, key );
                //foreach( var file in zipFiles ) {
                //    logger.LogInformation( "正在处理压缩文件：" + file.Name );
                //    File.Delete( file.FullName );
                //}

                logger.LogInformation( "解压完成。" + DateTime.Now );

                await Task.CompletedTask;
            }
            catch( Exception ex ) {
                logger.LogError( "Syinpo.Monitor.Processor 解压 错误：" + ex.Message );
            }
        }

        private static async Task Log( ILogger<Program> logger, MonitorOptions options ) {
            try {
                logger.LogInformation( "开始批量处理Request Log文件" );


                var files = ListFiles( Path.Combine( options.FindRequestLogPath, "extract" ) );
                foreach( var file in files ) {
                    logger.LogInformation( "正在处理Request Log文件：" + file.Name );

                    try
                    {
                        var path = CommonHelper.MapPath(file.FullName);
                        var data = MessagePackHelper.ToObject<List<HttpLog>>(path);
                        if (data.Any())
                        {
                            var dbContext = IoC.Resolve<IDbContext>("monitor_dbcontext");
                            var result = data.MapToList<RequestLog>();
                            await IoC.Resolve<IGenericRepository<RequestLog>>().CreateRange(result);  
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(  "Syinpo.Monitor.Processor Request Log 错误：" + ex.Message + ex.InnerException?.Message );
                    }

                    File.Delete( file.FullName );
                }

                logger.LogInformation( "结束这批处理Request Log文件，共:" + files.Count() );

                await Task.CompletedTask;
            }
            catch( Exception ex ) {
                logger.LogError( "Syinpo.Monitor.Processor Request Log 错误：" + ex.Message );
            }
        }

        private static MonitorConcurrentDictionary<TimeData> timeStore = new MonitorConcurrentDictionary<TimeData>();
        private static async Task Time( ILogger<Program> logger, MonitorOptions options ) {
            try {
                logger.LogInformation( "开始批量处理TimeData Log文件" );

                var files = ListFiles( Path.Combine( options.FindTimeDataLogPath, "extract" ) );
                foreach( var file in files ) {
                    logger.LogInformation( "正在处理Time Log文件：" + file.Name );

                    var path = CommonHelper.MapPath( file.FullName );
                    var data = MessagePackHelper.ToObject<List<TimeData>>( path );
                    if( data.Any() ) {
                        foreach( var g in data.GroupBy( g => g.RequestGroup ) ) {
                            timeStore.AddRang( g.Key, g.ToList() );
                        }
                        data = null;
                    }

                    File.Delete( file.FullName );
                }

                logger.LogInformation( "结束这批处理Time Log文件，共:" + files.Count() );
            }
            catch( Exception ex ) {
                logger.LogError( "Syinpo.Monitor.Processor Time Log 错误：" + ex.Message );
            }

            await Task.CompletedTask;
        }

        private static ConcurrentDictionary<string, string> dateStore = new ConcurrentDictionary<string, string>();
        private static async Task TimeStore( ILogger<Program> logger, MonitorOptions options ) {
            try {
                logger.LogInformation( "开始处理 TimeStore 数据" );

                var now = DateTimeHelper.GetTimeGroup( DateTime.Now );
                var keys = timeStore.GetKeys().Where( w => w != now ).ToList();
                foreach( var key in keys ) {
                    var logs = timeStore.GetByKey( key );
                    if( logs == null || !logs.Any() ) {
                        timeStore.Remove( key );
                        continue;
                    }
                    var trafficStatistService = IoC.Resolve<ITrafficStatistService>();

                    if( dateStore.ContainsKey( key ) ) {
                        var item = await trafficStatistService.GetTrafficStatistByDateTime( Convert.ToInt64( key ) );
                        if( item != null ) {
                            item.TotalHttpReq += logs.Count( w => w.Key == MonitorContextKeys.http );
                            item.TotalSignalrReq += logs.Count( w => w.Key == MonitorContextKeys.signalr );
                            item.TotalReq = item.TotalHttpReq + item.TotalSignalrReq;
                            item.ExceptionReq += logs.Count( w => w.Key == MonitorContextKeys.fault );
                            item.TotalSignalrConnectReq += logs.Count( w => w.Key == MonitorContextKeys.signalr_connect );
                            item.TotalSignalrDisconnectReq += logs.Count( w => w.Key == MonitorContextKeys.signalr_disconnect );

                            await trafficStatistService.UpdateTrafficStatist( item );

                            continue;
                        }
                        else {
                            logger.LogWarning( "Syinpo.Monitor.Processor TimeStore key 与实际入库不一致：" + key );
                        }
                    }


                    TrafficStatist trafficStatist = new TrafficStatist();

                    // request
                    trafficStatist.DateTime = Convert.ToInt64( key );
                    trafficStatist.TotalHttpReq = logs.Count( w => w.Key == MonitorContextKeys.http );
                    trafficStatist.TotalSignalrReq = logs.Count( w => w.Key == MonitorContextKeys.signalr );
                    trafficStatist.TotalReq = trafficStatist.TotalHttpReq + trafficStatist.TotalSignalrReq;
                    trafficStatist.ExceptionReq = logs.Count( w => w.Key == MonitorContextKeys.fault );

                    // signalr
                    trafficStatist.TotalSignalrConnectReq = logs.Count( w => w.Key == MonitorContextKeys.signalr_connect );
                    trafficStatist.TotalSignalrDisconnectReq = logs.Count( w => w.Key == MonitorContextKeys.signalr_disconnect );
                    var onlines = logs.Where( w => w.Key == MonitorContextKeys.online_device ).ToList();
                    if( onlines.Any() ) {
                        trafficStatist.TotalOnlineDevice = (long)onlines.Average( a => Convert.ToInt64( a.Val ) );
                    }

                    // server
                    var cpu_server = logs.Where( w => w.Key == MonitorContextKeys.cpu_server ).ToList();
                    if( cpu_server.Any() ) {
                        trafficStatist.ServerCpuAvgRate = cpu_server.Average( a => Convert.ToDecimal( a.Val ) );
                        trafficStatist.ServerCpuTopRate = cpu_server.Max( a => Convert.ToDecimal( a.Val ) );
                    }
                    var ram_server = logs.Where( w => w.Key == MonitorContextKeys.ram_server ).ToList();
                    if( ram_server.Any() ) {
                        trafficStatist.ServerRamAvgRate = ram_server.Average( a => Convert.ToDecimal( a.Val ) );
                        trafficStatist.ServerRamTopRate = ram_server.Max( a => Convert.ToDecimal( a.Val ) );
                    }

                    // app
                    var cpu_app = logs.Where( w => w.Key == MonitorContextKeys.cpu_app ).ToList();
                    if( cpu_app.Any() ) {
                        trafficStatist.AppliationCpuAvgRate = cpu_app.Average( a => Convert.ToDecimal( a.Val ) );
                        trafficStatist.AppliationCpuTopRate = cpu_app.Max( a => Convert.ToDecimal( a.Val ) );
                    }
                    var ram_app_size = logs.Where( w => w.Key == MonitorContextKeys.ram_app_size ).ToList();
                    if( ram_app_size.Any() ) {
                        trafficStatist.AppliationRamAvgSize = ram_app_size.Average( a => Convert.ToDecimal( a.Val ) );
                    }

                    //trafficStatist.AppliationRamAvgRate = logs
                    //    .Where( w => w.Item1 == MonitorContextKeys.ram_app )
                    //    .Average( a => (decimal)a.Item2 );
                    //trafficStatist.AppliationRamTopRate = logs
                    //    .Where( w => w.Item1 == MonitorContextKeys.ram_app )
                    //    .Max( a => (decimal)a.Item2 );


                    trafficStatist.CreateTime = DateTime.Now;

                    await trafficStatistService.InsertTrafficStatist( trafficStatist );
                    trafficStatistService = null;

                    timeStore.Remove( key );
                    if( !dateStore.ContainsKey( key ) ) {
                        dateStore.TryAdd( key, key );
                    }

                    if( dateStore.Keys.Count > 100 )
                    {
                        dateStore.TryRemove(dateStore.Keys.First(), out string val);
                    }
                }

                logger.LogInformation( "结束处理 TimeStore." );
            }
            catch( Exception ex ) {
                logger.LogError( "Syinpo.Monitor.Processor TimeStore 错误：" + ex.Message );
            }
        }

        public static IEnumerable<FileInfo> ListFiles( string path ) {
            if( !Directory.Exists( path ) ) {
                throw new ArgumentException( "目录不能存在： " + path );
            }

            return new DirectoryInfo( path )
                .GetFiles( "*", SearchOption.AllDirectories )
                //.Where( w => !w.Name.Contains( currentFile ) )
                .ToList();
        }
    }
}
