using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syinpo.Core.Domain.RestApi;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Syinpo.Unity.AspNetCore.Filters {
    public class AuthorizeCheckOperationFilter : IOperationFilter {
        public void Apply( Microsoft.OpenApi.Models.OpenApiOperation operation, OperationFilterContext context ) {
            // Check for authorize attribute
            var hasAuthorize = context.ApiDescription.CustomAttributes().OfType<AuthorizeAttribute>().Any() ||
                               context.ApiDescription.CustomAttributes().OfType<AuthorizeAttribute>().Any();

            if( !operation.Responses.ContainsKey( "-1" ) )
                operation.Responses.Add( "-1", new Microsoft.OpenApi.Models.OpenApiResponse { Description = "失败", } );

            if( hasAuthorize ) {
                operation.Responses.Add( "401", new Microsoft.OpenApi.Models.OpenApiResponse { Description = "认证失败" } );
                operation.Responses.Add( "403", new Microsoft.OpenApi.Models.OpenApiResponse { Description = "禁止访问" } );

                //operation.Security = new List<IDictionary<string, IEnumerable<string>>>();
                //operation.Security.Add( new Dictionary<string, IEnumerable<string>>
                //{
                //    { "oauth2", new [] { "api" } }
                //} );
            }
        }
    }
}
