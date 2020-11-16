using System;
using System.Collections.Generic;
using System.Text;
using OpenTelemetry.Trace.Configuration;
using OpenTelemetry.Trace.Export;

namespace Syinpo.Core.Monitor.TracerModule {
    public static class TracerBuilderExtensions {
        public static TracerBuilder UseMonitor( this TracerBuilder builder, IServiceProvider serviceProvider ) {
            if( builder == null ) {
                throw new ArgumentNullException( nameof( builder ) );
            }

            return builder.AddProcessorPipeline( b => b
                .SetExporter( serviceProvider.GetService( typeof( MonitorExporter ) ) as MonitorExporter )
                .SetExportingProcessor( e => new BatchingSpanProcessor( e ) ) );
        }
    }
}
