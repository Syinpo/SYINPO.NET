using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Syinpo.Core.Helpers;
using Syinpo.Core.Monitor.PackModule;
using Syinpo.Core.Monitor.TracerModule;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Syinpo.Core.Monitor {
    public class MonitorService : IMonitorService {
        private readonly ILogger<MonitorService> _logger;
        private readonly ConcurrentQueue<MonitorEvent> events = new ConcurrentQueue<MonitorEvent>();
        private readonly IOptions<MonitorOptions> _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public MonitorService( IHttpClientFactory httpClientFactory, ILogger<MonitorService> logger, IOptions<MonitorOptions> settings ) {
            _logger = logger;
            _settings = settings;
            _httpClientFactory = httpClientFactory;
        }

        public void QueueEvent( MonitorEvent ev ) {
            events.Enqueue( ev );
        }

        public async Task Flush() {
            while( true ) {
                var dequeueSize = events.Count < _settings.Value.BatchSize ?
                    events.Count :
                    _settings.Value.BatchSize;

                var chunk = events
                    .DequeueChunk( dequeueSize )
                    .ToList();

                if( !chunk.Any() )
                    break;

                var path = CommonHelper.MapPath( _settings.Value.ResponseStorePath + $"{CommonHelper.NewSequentialGuid().ToString()}.log" );
                MessagePackHelper.ToPack( path, chunk );

                await Task.CompletedTask;
            }
        }

        public async Task Zip() {
            var path = Path.Combine(
                CommonHelper.MapPath( _settings.Value.ResponseStorePath ),
                "zip",
                $"{CommonHelper.NewSequentialGuid().ToString()}.zip" );


            MonitorZipFileHelper.CreateFromDirectory(
                CommonHelper.MapPath( _settings.Value.ResponseStorePath ),
                path,
                true );

            await Task.CompletedTask;
        }

        public async Task SendBatch( IEnumerable<MonitorEvent> items ) {
            var client = _httpClientFactory.CreateClient( "monitor" );

            var message = new HttpRequestMessage();

            var content = JsonHelper.ToJson( items );
            message.Content = new StringContent( content, Encoding.UTF8, "application/json" );
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri( $"{_settings.Value.ServerAddress}api/batch/events" );

            var resp = await client.SendAsync( message );
            if( !resp.IsSuccessStatusCode )
                _logger.LogWarning( "MonitorService SendBatch erros:{StatusCode}", resp.StatusCode );
        }
    }
}
