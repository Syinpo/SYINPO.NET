using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.Jobs;
using Syinpo.BusinessLogic.Notifications.ReceiveHandlers;
using Syinpo.BusinessLogic.Notifications.SendHandlers;
using Syinpo.BusinessLogic.Safety;
using Syinpo.BusinessLogic.SignalR.Online;
using Syinpo.Core;
using Microsoft.AspNetCore.Mvc;
using Syinpo.BusinessLogic.SignalR.Hubs;
using Syinpo.BusinessLogic.SignalR.Notifications;
using Syinpo.BusinessLogic.Users;
using Syinpo.Core.Caches;
using Syinpo.Core.Data;
using Syinpo.Core.Domain.Poco;
using Syinpo.Core.Hangfire;
using Syinpo.Core.Helpers;
using Syinpo.Core.Reflection;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Syinpo.Model.Request;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Model;
using Syinpo.Model.Core.Notifications;
using Newtonsoft.Json.Linq;
using Syinpo.Unity.AspNetCore.Routes;
using Syinpo.Unity.Firewall.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using PipelineNet.ChainsOfResponsibility;
using PipelineNet.MiddlewareResolver;

namespace Syinpo.Admin.Api.Controllers {
    public partial class HomeController : Controller {

        #region 字段

        private readonly ICurrent _current;
        private readonly ITokenService _tokenService;
        private readonly IDeviceService _deviceService;
        private readonly ITypeFinder _typeFinder;
        private readonly ITypeResolve _typeResolve;
        private readonly IDbContext _dbContext;
        private readonly DbContextFactory _dbContextFactory;
        private readonly ICache _cache;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly HangfireOptions _hangfireOptions;
        private readonly IPushService _pushService;
        private readonly IRouteAnalyzer _routeAnalyzer;
        private readonly ILogger<HomeController> _logger;

        #endregion

        #region 构造函数

        public HomeController(
            ICurrent current,
            IPushService pushService,
            IRouteAnalyzer routeAnalyzer,
            IOnlineManager onlineManager,
            ILogger<HomeController> logger,
            ITokenService tokenService,
            IDeviceService deviceService,
            ITypeFinder typeFinder,
            ITypeResolve typeResolve,
            IDbContext dbContext,
            DbContextFactory dbContextFactory,
            ICache cache,
            IBackgroundJobManager backgroundJobManager ,
            IOptions<HangfireOptions> hangfireOption) {
            _current = current;
            _tokenService = tokenService;
            _deviceService = deviceService;
            _typeFinder = typeFinder;
            _typeResolve = typeResolve;
            _dbContext = dbContext;
            _dbContextFactory = dbContextFactory;
            _cache = cache;
            _backgroundJobManager = backgroundJobManager;
            _hangfireOptions = hangfireOption.Value;
            _pushService = pushService;
            _routeAnalyzer = routeAnalyzer;
            _logger = logger;
        }

        #endregion

        public async Task<IActionResult> Online() {
            var onlineDevices = IoC.Resolve<IOnlineManager>().GetAllClients().Where( c => c.DeviceId.HasValue )
             .ToImmutableList();

            var onlineUsers = IoC.Resolve<IOnlineManager>().GetAllClients().Where( c => c.UserId.HasValue )
                .ToImmutableList();

            var devices = new List<DeviceModel>();
            foreach( var item in onlineDevices ) {
                if( !item.DeviceId.HasValue )
                    continue;

                var deviceServ = IoC.Resolve<IDeviceService>();
                var device = await deviceServ.GetDeviceById( item.DeviceId.Value );

                if( device == null )
                    continue;

                var data = new DeviceModel {
                    Id = item.DeviceId.Value,
                    ConnectionId = item.ConnectionId,
                    ConnectTime = item.ConnectTime,
                    IpAddress = item.IpAddress,

                    DeviceUuid = device.DeviceUuid,
                    WeiXinName = string.Empty,

                    ServerName = item.ServerName,
                    SysName = item.SysName
                };
                devices.Add( data );
            }

            var users = new List<UserModel>();
            foreach( var item in onlineUsers ) {
                if( !item.UserId.HasValue )
                    continue;

                var userSer = IoC.Resolve<IUserService>();
                var user = await userSer.GetUserById( item.UserId.Value );
                if( user == null )
                    continue;

                var data = new UserModel {
                    Id = item.UserId.Value,
                    ConnectionId = item.ConnectionId,
                    ConnectTime = item.ConnectTime,
                    IpAddress = item.IpAddress,

                    UserName = user.Username,
                    WeiXin = ""
                };
                users.Add( data );
            }


            var all = new HomeData { Devices = devices, Users = users };



            return View( all );
        }


        public async Task<IActionResult> Index()
        {
           // await IoC.Resolve<EventQueueJob>().Execute( new NullJobArgs());

            await Task.CompletedTask;
            return View();
        }

        public async Task<IActionResult> Access()
        {
              await Task.CompletedTask;
            return View();
        }

        [HttpPost, ActionName( "Access" )]
        public virtual IActionResult Access_Submit( IFormCollection form ) {
            var model = _routeAnalyzer.GetAllRouteInformations().OrderBy( p => p.Controller ).ToList();

            foreach( var item in model ) {
                var key = "disable_" + item.Path;

                var disable = form.ContainsKey( key ) && form[ key ].ToString() == "on";

                RouteMemoryStore.Set(new Unity.Firewall.RouteItem {Route = item.Path, IsDisable = disable } );
            }


            return RedirectToAction( "Access" );
        }

        public class HomeData {
            public HomeData() {
                Devices = new List<DeviceModel>();
                Users = new List<UserModel>();
            }

            public List<DeviceModel> Devices {
                get; set;
            }

            public List<UserModel> Users {
                get; set;
            }

            public List<string> DeviceContections {
                get; set;
            }
        }


        public class DeviceModel {
            public int Id {
                get; set;
            }

            public string ConnectionId {
                get; set;
            }

            public DateTime ConnectTime {
                get; set;
            }

            public string IpAddress {
                get; set;
            }

            public string DeviceUuid {
                get; set;
            }

            public string WeiXinName {
                get; set;
            }
            public string ServerName {
                get; set;
            }

            public string SysName {
                get;set;
            }
        }

        public class UserModel {
            public int Id {
                get; set;
            }

            public string ConnectionId {
                get; set;
            }

            public DateTime ConnectTime {
                get; set;
            }

            public string IpAddress {
                get; set;
            }

            public string UserName {
                get; set;
            }

            public string WeiXin {
                get; set;
            }
        }
    }
}