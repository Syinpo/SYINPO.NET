using MessagePack;

namespace Syinpo.Core.Monitor.PackModule {
    [MessagePackObject( keyAsPropertyName: true )]
    public class TimeData {
        public TimeData( string requestGroup, string key, object val )
        {
            RequestGroup = requestGroup;
            Key = key;
            Val = val;
        }

        public string RequestGroup {
            get; set;
        }

        public string Key {
            get;
        }

        public object Val {
            get;
        }

    }
}
