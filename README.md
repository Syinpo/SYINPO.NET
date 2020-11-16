SYINPO.NET : 免费开源的万台设备通讯的后台解决方案 [![Build status](http://t.ifengke.com/wwwroot/images/github/master.svg)](https://ci.appveyor.com/project/Syinpo/Syinpo.Net/branch/master)
====

[Syinpo.Net](http://www.Syinpo.com/)是免费的Asp.Net Core移动设备通讯的解决方案.

Syinpo.Net 运行在.NET Core 3.1 和 SqlServer 2012 (或更高) 后端数据库.

Syinpo.Net 是跨平台的，您可以在 Windows、Linux 或 Mac 上运行它.

Syinpo.Net 体系架构使用最新的 Microsoft 技术, 提供高性能、安全性和稳定性. 并为对接的开发团队提供清晰而详细的协议接口文档。

特性:
------------------------
* Ef Core SqlServer 读写分析
* Ef Core SqlServer 二级缓存
* Cap: 分布式事件总线集群隔离方案
* SignalrBus: 后台系统之间通讯方案
* 设备请求流量防火墙
* 监控系统
* 接口文档一键生成器，支持生成Signalr接口


截图:
------------------------
* ![Doc](http://t.ifengke.com/wwwroot/images/github/doc.png)
* ![home](http://t.ifengke.com/wwwroot/images/github/home.png)

硬件(集群版):
------------------------
+ 2台SQL服务器（组建一个集群）  
64G内存+8核CPU+2TB硬盘

+ 2台Web服务器（组建一个集群）  
16G内存+4核CPU+512G硬盘

+ 1台Redis缓存服务器  
32G内存, 可共享Web和数据库服务器

+ 1台发布/订阅推送服务器  
16G内存, 可共享Web和数据库服务器

+ 1台负载平衡服务器  
16G内存，可Linux系统, 可共享Web和数据库服务器

程序设计:
------------------------
*   [框架](#)
    *   [C#](#)
    *   [ASP.NET Core](#)
    *   [Sql Server](#)
    *   [StackExchange Redis](#)
    *   [Entity Framework Core](#)
    *   [Dapper](#)
*   [数据](#)
    *   [日志](#)
        *   [日志：Serilog](#)
    *   [健康检查](#)
        *   [httpUnit](#)
    *   [度量统计](#)
        *   [统计分析](#)
        *   [机器学习](#)
*   [监控](#)
    *   [监控：Web Server](#)
    *   [监控：SQL Server](#)
    *   [监控：Redis](#)
    *   [监控：日志](#)
    *   [监控：异常](#)
    *   [监控：反向代理（负载均衡）](#)
    *   [监控：警报](#)
*   [性能分析](#)
    *   [客户时间](#)
    *   [移动端时间](#)
    *   [服务端时间](#)
    *   [MiniProfiler](#)
    *   [监控：SQL Server](#)
*   [弹性与灾备](#)
    *   [弹性：异常重试](#)
    *   [灾备：数据库故障转移](#)
    *   [灾备：Web服务器故障转移](#)
    *   [灾备：反向代理故障转移](#)
*   [部署](#)
    *   [部署：SQL Server](#)
    *   [部署：Web应用](#)
    *   [部署：计划任务](#)
*   [工具摘要](#)

贡献:
------------------------
只需要简单的发起pull请求.