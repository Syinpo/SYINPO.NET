using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Syinpo.Core.Reflection;
using Syinpo.Unity.Firewall.Rules;

namespace Syinpo.Unity.Firewall.Policy {
    public static class PolicyDefaultStore {
        public static ConcurrentDictionary<FirewallPolicyEnum, IFirewallRule> _cache = new ConcurrentDictionary<FirewallPolicyEnum, IFirewallRule>();

        static PolicyDefaultStore() {
            // 禁止访问
            _cache.TryAdd( FirewallPolicyEnum.ForbiddenAccessRule, new ForbiddenAccessRuleHandler( new FirewallOptions { } ) );

            // 配置型的禁止请求
            _cache.TryAdd( FirewallPolicyEnum.DisableRequestRule, new DisableRequestRuleHandler( new FirewallOptions { } ) );

            // 设备每10秒最多请求1次
            _cache.TryAdd( FirewallPolicyEnum.DeviceInTimeMaxRequestRule_s10_c1, new DeviceInTimeMaxRequestRuleHandler(
                new FirewallOptions {
                    TimeUnit = Unity.Firewall.TimeUnitEnum.Second,
                    TimeValue = 10,
                    MaxRequest = 1
                } ) );

            // 设备每1分钟最多请求5次
            _cache.TryAdd( FirewallPolicyEnum.DeviceInTimeMaxRequestRule_m1_c5, new DeviceInTimeMaxRequestRuleHandler(
                new FirewallOptions {
                    TimeUnit = Unity.Firewall.TimeUnitEnum.Minute,
                    TimeValue = 1,
                    MaxRequest = 5
                } ) );

            // 设备每1分钟最多请求1次
            _cache.TryAdd( FirewallPolicyEnum.DeviceInTimeMaxRequestRule_m1_c1, new DeviceInTimeMaxRequestRuleHandler(
                new FirewallOptions {
                    TimeUnit = Unity.Firewall.TimeUnitEnum.Minute,
                    TimeValue = 1,
                    MaxRequest = 2
                } ) );

            // 设备每1分钟最多请求30次
            _cache.TryAdd( FirewallPolicyEnum.DeviceInTimeMaxRequestRule_m1_c30, new DeviceInTimeMaxRequestRuleHandler(
                new FirewallOptions {
                    TimeUnit = Unity.Firewall.TimeUnitEnum.Minute,
                    TimeValue = 1,
                    MaxRequest = 30
                } ) );

            // 设备每5分钟最多请求1次
            _cache.TryAdd( FirewallPolicyEnum.DeviceInTimeMaxRequestRule_m5_c1, new DeviceInTimeMaxRequestRuleHandler(
                new FirewallOptions {
                    TimeUnit = Unity.Firewall.TimeUnitEnum.Minute,
                    TimeValue = 5,
                    MaxRequest = 1
                } ) );
        }

        public static IFirewallRule TryGet( FirewallPolicyEnum firewallPolicy ) {
            if( _cache.TryGetValue( firewallPolicy, out IFirewallRule firewallRule ) )
            {
               return (IFirewallRule)ReflectionUtils.CreateInstanceFromType(firewallRule.GetType(), firewallRule.GetOption());
            }

            return default(IFirewallRule);
        }
    }
}
