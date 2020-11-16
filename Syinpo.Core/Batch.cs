using System;
using Syinpo.Core.Monitor.PackModule;

namespace Syinpo.Core {
    public class Batch {
        private static Lazy<MonitorConcurrentDictionary<string>> lazyCallDevicePhones = new Lazy<MonitorConcurrentDictionary<string>>( () =>
        {
            return new MonitorConcurrentDictionary<string>();
        } );

        private static Lazy<MonitorConcurrentDictionary<string>> lazySmsDevicePhones = new Lazy<MonitorConcurrentDictionary<string>>( () =>
        {
            return new MonitorConcurrentDictionary<string>();
        } );

        public static MonitorConcurrentDictionary<string> CallDevicePhones {
            get {
                return lazyCallDevicePhones.Value;
            }
        }

        public static MonitorConcurrentDictionary<string> SmsDevicePhones {
            get {
                return lazySmsDevicePhones.Value;
            }
        }

    }
}
