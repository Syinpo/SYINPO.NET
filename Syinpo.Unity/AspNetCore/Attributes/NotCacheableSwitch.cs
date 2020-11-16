using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syinpo.Core.Data;
using Syinpo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Syinpo.Unity.AspNetCore.Attributes {
    public class NotCacheableSwitch : TypeFilterAttribute {

        public bool IsWrite {
            get; set;
        }


        public NotCacheableSwitch( bool isWrite ) : base( typeof( NotCacheableSwitchFilter ) ) {
            IsWrite = isWrite;
        }


        private class NotCacheableSwitchFilter : IActionFilter {
            private readonly ICurrent _current;
            private readonly DbContextFactory _dbContextFactory;
            private readonly IDatabaseHelper _databaseHelper;

            public NotCacheableSwitchFilter( ICurrent current, DbContextFactory dbContextFactory, IDatabaseHelper databaseHelper ) {
                _current = current;
                _dbContextFactory = dbContextFactory;
                _databaseHelper = databaseHelper;
            }

            public void OnActionExecuting( ActionExecutingContext actionExecutingContext ) {
                if( actionExecutingContext == null )
                    throw new ArgumentNullException( nameof( actionExecutingContext ) );

                if( actionExecutingContext.HttpContext.Request == null )
                    return;

                var actionFilter = actionExecutingContext.ActionDescriptor.FilterDescriptors
                    .Where( filterDescriptor => filterDescriptor.Scope == FilterScope.Action || filterDescriptor.Scope == FilterScope.Controller )
                    .Select( filterDescriptor => filterDescriptor.Filter )
                    .OfType<NotCacheableSwitch>()
                    .LastOrDefault();
                if( actionFilter == null )
                    return;

                //if( actionFilter.IsWrite ) {
                //    _dbContextFactory.IsTransaction = true;
                //}
                _databaseHelper.NotCacheable = true;
            }


            public void OnActionExecuted( ActionExecutedContext context ) {
            }

        }

    }
}
