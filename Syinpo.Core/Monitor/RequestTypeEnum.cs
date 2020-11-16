using System.ComponentModel.DataAnnotations;

namespace Syinpo.Core.Monitor {
    public enum RequestTypeEnum {
        [Display( Name = "http" )]
        Http = 10,

        [Display( Name = "signalr" )]
        Signalr = 20,

        [Display( Name = "signalr_connect" )]
        Signalr_Connect = 22,

        [Display( Name = "signalr_disconnect" )]
        Signalr_Disconnect = 24,

        [Display( Name = "sql" )]
        Sql = 30,
    }
}
