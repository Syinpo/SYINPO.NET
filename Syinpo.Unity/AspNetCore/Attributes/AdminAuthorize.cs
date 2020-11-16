using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Syinpo.BusinessLogic.Safety;
using Syinpo.BusinessLogic.Users;
using Syinpo.Core;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Syinpo.Unity.AspNetCore.Attributes {
    public class AdminAuthorize : TypeFilterAttribute {

        public AdminAuthorize() : base( typeof( AdminAuthorizeFilter ) ) {
        }


        private class AdminAuthorizeFilter : IAuthorizationFilter {
            #region 字段
            private readonly ICurrent _current;
            private readonly IUserService _userService;
            #endregion

            #region 构造函数

            public AdminAuthorizeFilter( ICurrent current, IUserService userService) {
                _current = current;
                _userService = userService;
            }

            #endregion

            #region 方法


            public void OnAuthorization( AuthorizationFilterContext filterContext ) {
                if( filterContext == null )
                    throw new ArgumentNullException( nameof( filterContext ) );

                var actionFilter = filterContext.ActionDescriptor.FilterDescriptors
                    .Where( filterDescriptor => filterDescriptor.Scope == FilterScope.Action || filterDescriptor.Scope == FilterScope.Controller )
                    .Select( filterDescriptor => filterDescriptor.Filter )
                    .OfType<AdminAuthorize>()
                    .LastOrDefault();

                if( actionFilter != null ) {
                    try {

                        bool success = false;

                        if( _current.User == null )
                            throw new SysException( 401, "用户验证" );

                        var user = _userService.GetUserById( _current.User.Id ).ConfigureAwait( false ).GetAwaiter().GetResult();
                        if( user == null )
                            throw new SysException( 401, "用户验证" );

                        if( user.IsAdmin ) {
                            success = true;
                        }


                        if( !success ) {
                            throw new SysException( 401, "非超级管理员或者独立部署管理员用户" );
                        }
                    }
                    catch( Exception ex ) {
                        var errorObject = new ErrorObject {
                            Code = 500,
                            Message = ex.Message
                        };

                        if( ex is SysException ) {

                            errorObject.Code = ( ex as SysException ).Code;
                        }

                        //var errorsJson = JsonHelper.ToJson2( errorObject );
                        filterContext.Result = new JsonResult( errorObject );
                    }

                }
            }

            #endregion
        }
    }
}
