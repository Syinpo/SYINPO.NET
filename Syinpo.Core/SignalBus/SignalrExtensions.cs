using Syinpo.Core.SignalBus.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Syinpo.Core.SignalBus {
    public static class SignalrExtensions {
        public static IServiceCollection AddSignalrBus( this IServiceCollection services ) {
            services.AddHostedService<SignalrClientWorker>();

            return services;
        }

        public static IServiceCollection AddSignalrBusServer( this IServiceCollection services ) {
            return services;
        }
    }
}
