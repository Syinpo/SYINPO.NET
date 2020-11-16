using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Syinpo.Core.SignalBus.Model {
    public enum SignalrMessageTypeEnum:int {

        [Display( Name = "推送" )]
        Push = 10,

        [Display( Name = "未知" )]
        Othor = 30,
    }
}
