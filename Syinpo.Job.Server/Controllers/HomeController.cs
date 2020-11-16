using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.Safety;
using Syinpo.BusinessLogic.SignalR.Notifications;
using Syinpo.BusinessLogic.SignalR.Online;
using Syinpo.BusinessLogic.Users;
using Syinpo.Core;
using Syinpo.Core.Caches;
using Syinpo.Core.Data;
using Syinpo.Core.Hangfire;
using Syinpo.Core.Reflection;
using Syinpo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Syinpo.Job.Server.Controllers {
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

        #endregion

        #region 构造函数

        public HomeController(
            ICurrent current,
            IOnlineManager onlineManager,
            ILogger<HomeController> logger,
            ITokenService tokenService,
            IDeviceService deviceService,
            ITypeFinder typeFinder,
            ITypeResolve typeResolve,
            IDbContext dbContext,
            DbContextFactory dbContextFactory,
            ICache cache,
            IBackgroundJobManager backgroundJobManager) {
            _current = current;
            _tokenService = tokenService;
            _deviceService = deviceService;
            _typeFinder = typeFinder;
            _typeResolve = typeResolve;
            _dbContext = dbContext;
            _dbContextFactory = dbContextFactory;
            _cache = cache;
            _backgroundJobManager = backgroundJobManager;
        }

        #endregion

        public async Task<IActionResult> Index() {
            await Task.CompletedTask;
            return View();
        }
    }
}