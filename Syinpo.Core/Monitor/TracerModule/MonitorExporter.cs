using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Syinpo.Core.Monitor.PackModule;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace.Export;

namespace Syinpo.Core.Monitor.TracerModule {
    public class MonitorExporter : SpanExporter {
        private readonly IOptions<MonitorOptions> _settings;
        private readonly IPackStore<MonitorEvent> _monitorService;
        private readonly ILogger<MonitorExporter> _logger;

        public MonitorExporter( IOptions<MonitorOptions> settings, IPackStore<MonitorEvent> monitorService, ILogger<MonitorExporter> logger ) {
            _settings = settings;
            _monitorService = monitorService;
            _logger = logger;
        }

        public async override Task<ExportResult> ExportAsync( IEnumerable<SpanData> batch, CancellationToken cancellationToken ) {
            try {
                if( !_settings.Value.UseMonitor )
                    return ExportResult.Success;

                var batchEvents = new List<MonitorEvent>();

                foreach( var span in batch ) {
                    if( span.Attributes.Any( a =>
                         a.Key == "http.url" && ( a.Value is string urlStr && urlStr.Contains( ":5003" ) ) ) )
                        continue;

                    var events = GenerateEvent( span );
                    batchEvents.AddRange( events );
                }

                foreach( var item in batchEvents ) {
                    _monitorService.AddQueue( item );
                }

                await Task.CompletedTask;

                return ExportResult.Success;
            }
            catch( Exception ex ) {
                _logger.LogError( "MonitorExporter error:" + ex.Message );
                return ExportResult.FailedNotRetryable;
            }
        }

        private IEnumerable<MonitorEvent> GenerateEvent( SpanData span ) {
            var list = new List<MonitorEvent>();

            string eventType = "un";
            if( span.Attributes.Any( a => a.Key == MonitorKeys.request_type && ( a.Value is string mt && mt.Contains( RequestTypeEnum.Http.ToString().ToLowerInvariant() ) ) ) ) {
                eventType = RequestTypeEnum.Signalr.ToString().ToLowerInvariant();
            }
            else if( span.Attributes.Any( a => a.Key == MonitorKeys.request_type && ( a.Value is string mt && mt.Contains( RequestTypeEnum.Signalr.ToString().ToLowerInvariant() ) ) ) ) {
                eventType = RequestTypeEnum.Signalr.ToString().ToLowerInvariant();
            }
            else if( span.Attributes.Any( a => a.Key == MonitorKeys.request_type && ( a.Value is string mt && mt.Contains( RequestTypeEnum.Sql.ToString().ToLowerInvariant() ) ) ) ) {
                eventType = RequestTypeEnum.Sql.ToString().ToLowerInvariant();
            }

            var ev = new MonitorEvent {
                EventType = eventType,
                EventStartTime = span.StartTimestamp.DateTime,
                DataSetName = _settings.Value.DefaultDataSet
            };
            var baseAttributes = new Dictionary<string, object>
            {
                { MonitorKeys.trace_id, span.Context.TraceId.ToString() },
                { MonitorKeys.service_name, span.Name }
            };
            if( span.ParentSpanId.ToString() != "0000000000000000" )
                ev.Data.Add( MonitorKeys.trace_parentid, span.ParentSpanId.ToString() );

            ev.Data.AddRange( baseAttributes );
            ev.Data.Add( MonitorKeys.trace_spanid, span.Context.SpanId.ToString() );
            ev.Data.Add( MonitorKeys.trace_duration, ( span.EndTimestamp - span.StartTimestamp ).TotalMilliseconds );

            foreach( var label in span.Attributes ) {
                if( !ev.Data.ContainsKey( label.Key ) )
                    ev.Data.Add( label.Key, label.Value.ToString() );
            }

            foreach( var attr in span.LibraryResource.Attributes ) {
                if( !ev.Data.ContainsKey( attr.Key ) )
                    ev.Data.Add( attr.Key, attr.Value );
            }

            foreach( var message in span.Events ) {
                var messageEvent = new MonitorEvent {
                    EventStartTime = message.Timestamp.DateTime,
                    DataSetName = _settings.Value.DefaultDataSet,
                    Data = message.Attributes.ToDictionary( a => a.Key, a => a.Value )
                };
                messageEvent.EventType = "trace_event";
                messageEvent.Data.Add( MonitorKeys.trace_parentid, span.Context.SpanId.ToString() );
                messageEvent.Data.Add( MonitorKeys.trace_spanevent, message.Name );
                messageEvent.Data.AddRange( baseAttributes );
                list.Add( messageEvent );
            }

            foreach( var link in span.Links ) {
                var linkEvent = new MonitorEvent {
                    EventStartTime = span.StartTimestamp.DateTime,
                    DataSetName = _settings.Value.DefaultDataSet,
                    Data = link.Attributes.ToDictionary( a => a.Key, a => a.Value )
                };
                linkEvent.EventType = "trace_link";
                linkEvent.Data.Add( MonitorKeys.trace_spanid, link.Context.SpanId.ToString() );
                linkEvent.Data.Add( MonitorKeys.trace_id, link.Context.TraceId.ToString() );
                linkEvent.Data.AddRange( baseAttributes );
                list.Add( linkEvent );
            }

            list.Add( ev );
            return list;
        }

        public override Task ShutdownAsync( CancellationToken cancellationToken ) {
            return Task.CompletedTask;
        }
    }

    public static class DictionaryExtensions {
        public static void AddRange<T, T1>( this Dictionary<T, T1> dest, Dictionary<T, T1> source ) {
            foreach( var kvp in source )
                dest.Add( kvp.Key, kvp.Value );
        }
    }

}
