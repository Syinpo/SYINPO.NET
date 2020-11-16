using System;
using System.Collections.Generic;
using System.Text;
using Syinpo.Unity.AspNetCore.Attributes;

namespace Syinpo.Unity.AspNetCore.Controllers {
    [NotCacheableSwitch(true)]
    public class BaseConsoleApiController : BaseApiController {
    }
}
