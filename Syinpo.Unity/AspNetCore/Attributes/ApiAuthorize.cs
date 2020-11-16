using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syinpo.BusinessLogic.Safety;
using Syinpo.Core.Domain.Poco;
using Microsoft.AspNetCore.Authorization;

namespace Syinpo.Unity.AspNetCore.Attributes {
    /// <summary>
    /// https://stackoverflow.com/questions/31464359/how-do-you-create-a-custom-authorizeattribute-in-asp-net-core
    /// </summary>
    public class ApiAuthorize : AuthorizeAttribute {
        public string Permission {
            get; set;
        }

        public ApiAuthorize() : base( "Permission" ) {
            AuthenticationSchemes = "Bearer";
        }
    }
}
