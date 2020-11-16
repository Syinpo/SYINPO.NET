using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Syinpo.Core.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Syinpo.Core.Monitor.PackModule {
    public class PackStore<T> : IPackStore<T> where T : class {
        private readonly ILogger<PackStore<T>> _logger;
        private readonly IOptions<MonitorOptions> _settings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly PackStoreOptions _packStoreOptions;

        private readonly ConcurrentQueue<T> events = new ConcurrentQueue<T>();

        public PackStore(
            IHttpClientFactory httpClientFactory,
            ILogger<PackStore<T>> logger,
            IOptions<MonitorOptions> settings,
            PackStoreOptions packStoreOptions ) {
            _settings = settings;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _packStoreOptions = packStoreOptions;
        }


        public void AddQueue( T log ) {
            events.Enqueue( log );
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


                var path = Path.Combine(
                    CommonHelper.MapPath( _packStoreOptions.TempStorePath ),
                    $"{CommonHelper.NewSequentialGuid().ToString()}.log" );
                MessagePackHelper.ToPack( path, chunk );

                await Task.CompletedTask;
            }
        }

        public async Task Zip() {
            var path = Path.Combine(
                CommonHelper.MapPath( _packStoreOptions.TempStorePath ),
                "zip",
                $"{CommonHelper.NewSequentialGuid().ToString()}.zip" );


            MonitorZipFileHelper.CreateFromDirectory(
                CommonHelper.MapPath( _packStoreOptions.TempStorePath ),
                path,
                true );

            await Task.CompletedTask;
        }

        public async Task UnZip() {
            var path1 = Path.Combine(
                CommonHelper.MapPath( _packStoreOptions.TempStorePath ),
                "zip" );

            var path2 = Path.Combine(
                CommonHelper.MapPath( _packStoreOptions.TempStorePath ),
                "extract" );

            MonitorZipFileHelper.UnFromDirectory( path1, path2 );

            await Task.CompletedTask;
        }


        public async Task SendBatch() {
            var path = Path.Combine( CommonHelper.MapPath( _packStoreOptions.TempStorePath ), "zip" );
            DirectoryInfo di = new DirectoryInfo( path );
            if (di.GetFiles("*.zip").Length == 0)
                return;

            var client = _httpClientFactory.CreateClient( "monitor" );

            using( var content = new MultipartFormDataContent() ) {
                var message = new HttpRequestMessage();
                message.Method = HttpMethod.Post;
                message.RequestUri = new Uri( $"{_settings.Value.ServerAddress}{_packStoreOptions.GatewayApiRoute}" );


                foreach( FileSystemInfo file in di.EnumerateFileSystemInfos( "*.zip", SearchOption.TopDirectoryOnly ) ) {
                    var myfilename = Path.GetFileName( file.FullName );
                    var fs = File.ReadAllBytes( file.FullName );

                    var streamContent = new StreamContent( new MemoryStream( fs ) );
                    streamContent.Headers.Add( "Content-Type", "application/zip" );
                    streamContent.Headers.Add( "Content-Disposition", "form-data; name=\"files\"; filename=\"" + myfilename + "\"" );

                    content.Add( streamContent, "file", file.Name );

                    file.Delete();
                }

                message.Content = content;
                var resp = await client.SendAsync( message );
                if( !resp.IsSuccessStatusCode )
                    _logger.LogWarning( "MonitorService SendBatch erros:{StatusCode}", resp.StatusCode );
            }
        }
    }


    static class QueueExtensions {
        internal static IEnumerable<T> DequeueChunk<T>( this ConcurrentQueue<T> queue, int chunkSize ) {
            for( int i = 0; i < chunkSize && queue.Count > 0; i++ ) {
                queue.TryDequeue( out T item );
                yield return item;
            }
        }
    }
}
