using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hangfire.Server;
using Syinpo.Core.SignalBus.Model;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Syinpo.Core.SignalBus.Client {
    public class SignalrClientWorker : BackgroundService {
        private readonly ILogger<SignalrClientWorker> _logger;
        private readonly SignalrClient _signalrClient;
        private readonly SysOptions _options;

        public SignalrClientWorker( ILogger<SignalrClientWorker> logger, SignalrClient signalrClient, IOptions<SysOptions> options ) {
            _logger = logger;
            _options = options?.Value;
            _signalrClient = signalrClient;
        }

        protected override async Task ExecuteAsync( CancellationToken stoppingToken ) {
            while( true ) {
                _logger.LogInformation( "SignalrClientWorker running at: {Time}", DateTime.Now );

                try {
                    await _signalrClient.Connect();

                    if( _signalrClient.Status == EntitySignal.Client.Enums.SignalrStatus.Connected ) {
                        break;
                    }

                    await Task.Delay( 1000 );
                }
                catch( Exception ex )
                {
                    _logger.LogWarning(ex, "SignalrClientWorker error");

                    await Task.Delay( 1000 );
                }
            }
        }

        public Task StopAsync( CancellationToken cancellationToken ) {
            return _signalrClient.Dispose();
        }
    }
}
