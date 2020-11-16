using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syinpo.Unity.AspNetCore.Attributes;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Syinpo.Unity.AspNetCore.Routes {
    public class RouteAnalyzer : IRouteAnalyzer {
        private readonly IActionDescriptorCollectionProvider m_actionDescriptorCollectionProvider;

        public RouteAnalyzer( IActionDescriptorCollectionProvider actionDescriptorCollectionProvider ) {
            m_actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public IEnumerable<RouteInformation> GetAllRouteInformations() {
            List<RouteInformation> ret = new List<RouteInformation>();

            var routes = m_actionDescriptorCollectionProvider.ActionDescriptors.Items;

            routes = routes.Where(w => w.FilterDescriptors.Any(f =>
                    f.Filter.GetType() == typeof(FirewallValidate) &&
                    ((FirewallValidate)f.Filter).Policies.Contains(Firewall.Policy.FirewallPolicyEnum
                        .DisableRequestRule)))
                .ToList();

            foreach( ActionDescriptor _e in routes ) {
                RouteInformation info = new RouteInformation();

                // Action
                if( _e.RouteValues.ContainsKey( "Action" ) ) {
                    info.Action = _e.RouteValues[ "Action" ];
                }

                if( _e.RouteValues.ContainsKey( "Controller" ) ) {
                    info.Controller = _e.RouteValues[ "Controller" ];
                }

                // Area
                if( _e.RouteValues.ContainsKey( "area" ) ) {
                    info.Area = _e.RouteValues[ "area" ];
                }

                // Path and Invocation of Razor Pages
                if( _e is PageActionDescriptor ) {
                    var e = _e as PageActionDescriptor;
                    info.Path = e.ViewEnginePath;
                    info.Invocation = e.RelativePath;
                }

                // Path of Route Attribute
                if( _e.AttributeRouteInfo != null ) {
                    var e = _e;
                    info.Path = $"/{e.AttributeRouteInfo.Template}";
                }

                // Path and Invocation of Controller/Action
                if( _e is ControllerActionDescriptor ) {
                    var e = _e as ControllerActionDescriptor;
                    if( info.Path == "" ) {
                        info.Path = $"/{e.ControllerName}/{e.ActionName}";
                    }
                    info.Invocation = $"{e.ControllerName}Controller.{e.ActionName}";
                }

                // Extract HTTP Verb
                if( _e.ActionConstraints != null && _e.ActionConstraints.Select( t => t.GetType() ).Contains( typeof( HttpMethodActionConstraint ) ) ) {
                    HttpMethodActionConstraint httpMethodAction =
                        _e.ActionConstraints.FirstOrDefault( a => a.GetType() == typeof( HttpMethodActionConstraint ) ) as HttpMethodActionConstraint;

                    if( httpMethodAction != null ) {
                        info.HttpMethod = string.Join( ",", httpMethodAction.HttpMethods );
                    }
                }



                // Additional information of invocation
                info.Invocation += $" ({_e.DisplayName})";

                // Generating List
                ret.Add( info );
            }

            // Result
            return ret;
        }
    }
}
