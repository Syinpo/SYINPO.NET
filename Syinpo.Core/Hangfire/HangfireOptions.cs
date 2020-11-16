using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Hangfire
{
    public class HangfireOptions
    {
        public bool UseHangfire { get; set; } = true;
        public bool UseHangfireServer { get; set; } = false;

        public string TestJobDeviceUuid {
            get;set;
        }
    }
}
