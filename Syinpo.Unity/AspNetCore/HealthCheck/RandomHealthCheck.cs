using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.Jobs;
using Syinpo.Core;
using Syinpo.Core.Data;
using Syinpo.Core.Domain.Poco;
using Syinpo.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Syinpo.Unity.AspNetCore.HealthCheck {
    public class RandomHealthCheck : IHealthCheck {

        public Task<HealthCheckResult> CheckHealthAsync( HealthCheckContext context, CancellationToken cancellationToken = default ) {
            return Task.FromResult( HealthCheckResult.Healthy() );
        }

    }

    public class HealthCheckWorker : BackgroundService {

        private readonly SysOptions _options;
        private readonly ILogger<HealthCheckWorker> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HealthCheckWorker( IHttpClientFactory httpClientFactory, IOptions<SysOptions> options, ILogger<HealthCheckWorker> logger ) {
            _logger = logger;
            _options = options?.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task Execute() {
            try
            {
                var client = _httpClientFactory.CreateClient("monitor");
                {
                    string url = _options?.LocalDomain + "health";
                    var a = await client.GetStringAsync( url );

                    client.Dispose();
                }
            }
            catch( Exception ex ) {
                _logger.LogError( ex, "HealthCheckWorker error:" + _options?.LocalDomain );

            }
        }


        protected async override Task ExecuteAsync( CancellationToken cancellationToken ) {
            while( true ) {
                await Execute();

                await Task.Delay( 1000 * 30 );
            }
        }
    }
}
