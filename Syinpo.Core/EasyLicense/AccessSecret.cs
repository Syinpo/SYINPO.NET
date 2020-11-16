using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.EasyLicense
{
    public class AccessSecret
    {
        public string Key { get; set; }

        public DateTime ExpiresTime { get; set; }

        public int Version { get; set; }
    }
}
