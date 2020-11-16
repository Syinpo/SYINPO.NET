using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.SignalR.Online;
using Syinpo.Core.Helpers;
using Syinpo.Core.Monitor;
using Syinpo.Core.Monitor.ClrModule;
using Syinpo.Core.Monitor.PackModule;
using Syinpo.Core.Monitor.TracerModule;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Syinpo.BusinessLogic.Monitor {
    public class MonitorWorker : BackgroundService {
        private readonly IPackStore<HttpLog> _httpLogStore;
        private readonly IPackStore<MonitorEvent> _monitorService;
        private readonly IPackStore<TimeData> _monitorTimeDataManager;
        private readonly ILogger<MonitorWorker> _logger;
        private readonly IOptions<MonitorOptions> _settings;
        private readonly IOnlineManager _onlineManager;

        public MonitorWorker( IPackStore<HttpLog> httpLogStore,
            IPackStore<MonitorEvent> monitorService,
            IPackStore<TimeData> monitorTimeDataManager,
            ILogger<MonitorWorker> logger,
            IOptions<MonitorOptions> settings,
            IOnlineManager onlineManager ) {
            _httpLogStore = httpLogStore;
            _monitorService = monitorService;
            _monitorTimeDataManager = monitorTimeDataManager;
            _logger = logger;
            _settings = settings;
            _onlineManager = onlineManager;
        }

        protected async override Task ExecuteAsync( CancellationToken cancellationToken ) {
            while( !cancellationToken.IsCancellationRequested ) {
                await LogExecute( cancellationToken );

                await EventExecute( cancellationToken );

                await ClrExecute( cancellationToken );

                await TimeDataExecute( cancellationToken );

                await Task.Delay( _settings.Value.BatchInterval, cancellationToken );
            }
        }

        private async Task LogExecute( CancellationToken cancellationToken ) {
            try {
                _logger.LogTrace( $"Starting MonitorWorker" );
                await _httpLogStore.Flush();

                await _httpLogStore.Zip();

                await _httpLogStore.SendBatch();
            }
            catch( Exception exception ) {
                _logger.LogError( "LogExecute ´íÎó." + exception.Message, exception );
            }
        }

        private async Task EventExecute( CancellationToken cancellationToken ) {
            try {
                await _monitorService.Flush();

                await _monitorService.Zip();

                await _monitorService.SendBatch();
            }
            catch( Exception exception ) {
                _logger.LogError( exception, "EventExecute ´íÎó." + exception.Message );
            }
        }

        private async Task ClrExecute( CancellationToken cancellationToken ) {
            try {
                var group = DateTimeHelper.GetTimeGroup( DateTime.Now );

                _monitorTimeDataManager.AddQueue( new TimeData( group, MonitorContextKeys.cpu_app, CpuHelpers.UsagePercent ) );
                _monitorTimeDataManager.AddQueue( new TimeData( group, MonitorContextKeys.cpu_server, CpuHelpers.TotalCpuUsagePercent ) );
                _monitorTimeDataManager.AddQueue( new TimeData( group, MonitorContextKeys.ram_app_size, GCHelpers.TotalMemory ) );
                _monitorTimeDataManager.AddQueue( new TimeData( group, MonitorContextKeys.ram_server, CpuHelpers.TotalRamUsagePercent ) );
                _monitorTimeDataManager.AddQueue( new TimeData( group, MonitorContextKeys.online_device,
                    _onlineManager.GetAllClients().Count( w => w.DeviceId.HasValue ) ) );

                await Task.CompletedTask;
            }
            catch( Exception exception ) {
                _logger.LogError( exception, "CLR Í³¼Æ´íÎó." );
            }
        }

        private async Task TimeDataExecute( CancellationToken cancellationToken ) {
            try {
                await _monitorTimeDataManager.Flush();

                await _monitorTimeDataManager.Zip();

                await _monitorTimeDataManager.SendBatch();
            }
            catch( Exception exception ) {
                _logger.LogError( exception, "TimeData ´íÎó." + exception.Message );
            }
        }

        public override async Task StopAsync( CancellationToken cancellationToken ) {
            _logger.LogTrace( $"End MonitorWorker" );

            await _httpLogStore.Flush();
            await _monitorService.Flush();
            await _monitorTimeDataManager.Flush();

            await base.StopAsync( cancellationToken );
        }
    }
}