using System;
using System.Collections.Generic;
using System.Text;
using Syinpo.Unity.AspNetCore.Attributes;

namespace Syinpo.Unity.AspNetCore.Controllers {
    [AdminAuthorize]
    [NotCacheableSwitch( true )]
    public class BaseAdminApiController : BaseApiController {
    }
}
