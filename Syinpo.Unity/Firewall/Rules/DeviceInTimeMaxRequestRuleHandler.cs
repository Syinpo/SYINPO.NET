using System;
using System.Linq;
using Syinpo.Core.Caches;
using Syinpo.Core.Helpers;
using Syinpo.Model;
using Syinpo.Unity.Firewall.Store;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Syinpo.Unity.Firewall.Rules {
    public class DeviceInTimeMaxRequestRuleHandler : IFirewallRule {
        private readonly FirewallOptions _optisons;
        private ICache _cache;
        private ICurrent _current;
        private ActionExecutingContext _actionExecutingContext;

        public DeviceInTimeMaxRequestRuleHandler( FirewallOptions optisons ) {
            _optisons = optisons;
        }


        public bool IsAllowed() {
            if( _actionExecutingContext == null || _cache == null || _current == null )
                return true;

            // 获取路由
            var actionDescriptor = _actionExecutingContext.ActionDescriptor as ControllerActionDescriptor;
            var actionName = actionDescriptor?.ActionName;
            var controllerName = actionDescriptor?.ControllerName;

            if( string.IsNullOrEmpty( actionName ) || string.IsNullOrEmpty( controllerName ) )
                return true;

            var route = $"/{controllerName}/{actionName}";
            var device = _current.Device;
            if( device == null )
                return false;

            ThrottlerItem throttlerItem = new ThrottlerItem {
                Identity = device.DeviceUuid,
                Route = route,
                RuleName = nameof( DeviceInTimeMaxRequestRuleHandler ),
                OptionKey = _optisons.GenerateKey()
            };
            throttlerItem = ThrottlerMemoryStore.Get( throttlerItem );


            var startTime = DateTime.Now;
            switch( _optisons.TimeUnit ) {
                case TimeUnitEnum.Second: {

                        startTime = DateTime.Now.AddSeconds( -_optisons.TimeValue );
                        break;
                    }
                case TimeUnitEnum.Minute: {

                        startTime = DateTime.Now.AddMinutes( -_optisons.TimeValue );
                        break;
                    }
                case TimeUnitEnum.Hours: {

                        startTime = DateTime.Now.AddHours( -_optisons.TimeValue );
                        break;
                    }
                case TimeUnitEnum.Day: {

                        startTime = DateTime.Now.AddDays( -_optisons.TimeValue );
                        break;
                    }
                case TimeUnitEnum.Today: {

                        startTime = DateTimeHelper.GetDayStart( startTime );
                        break;
                    }
            }

            bool isAllowed = true;

            // 检查多少时间内有多少个请求
            if( throttlerItem.LastRequests.All( i => i > startTime ) ) {
                if( throttlerItem.LastRequests.Count >= _optisons.MaxRequest ) {
                    // 请求太多
                    isAllowed = false;
                }
            }


            // 添加当前请求队列
            if( isAllowed ) {
                throttlerItem.LastRequestTime = DateTime.Now;
                // 清除无需计算得队列
                if( throttlerItem.LastRequests.Count >= _optisons.MaxRequest ) {
                    DateTime dequeueItem;
                    throttlerItem.LastRequests.TryDequeue( out dequeueItem );
                }
                throttlerItem.LastRequests.Enqueue( throttlerItem.LastRequestTime );
                ThrottlerMemoryStore.Set( throttlerItem );
            }

            return isAllowed;
        }

        public void Init( ActionExecutingContext actionExecutingContext, ICache cache, ICurrent current ) {
            _actionExecutingContext = actionExecutingContext;
            _cache = cache;
            _current = current;
        }

        public object GetOption()
        {
            return _optisons;
        }
    }
}
