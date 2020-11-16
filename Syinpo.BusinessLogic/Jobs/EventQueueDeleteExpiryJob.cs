using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Syinpo.BusinessLogic.Content;
using Syinpo.BusinessLogic.Devices;
using Syinpo.Core.Caches;
using Syinpo.Core.Container;
using Syinpo.Core.Data;
using Syinpo.Core.Hangfire;
using Syinpo.Core.Reflection;
using Syinpo.Model;
using Microsoft.Extensions.Logging;

namespace Syinpo.BusinessLogic.Jobs {

    public class EventQueueDeleteExpiryJob : IBackgroundJob<NullJobArgs> {

        private readonly ILogger<EventQueueJob> _logger;
        private readonly ICache _cache;
        private readonly ICurrent _current;
        private readonly IDataContainer _dataContainer;
        private readonly ITypeFinder _typeFinder;
        private readonly ITypeResolve _typeResolve;
        private readonly IDeviceService _deviceService;
        private readonly IDbContext _dbContext;
        private readonly IEventQueueService _eventQueueService;
        private readonly ICapPublisher _capBus;

        private volatile bool _isRuning;
        private readonly object _lockObj = new object();

        #region 常量

        private const string CurrentDeviceId = "currentDeviceId";
        private const string TraceId = "traceId";
        private const string CapMsgName = "cap-msg-name";
        private const string CustomVersion = "customVersion";
        private const string ItemId = "ItemId";

        #endregion

        public EventQueueDeleteExpiryJob( ILogger<EventQueueJob> logger, ICache cache, ICurrent current, IDataContainer dataContainer, ITypeFinder typeFinder, ITypeResolve typeResolve, IDeviceService deviceService, IDbContext dbContext, IEventQueueService eventQueueService, ICapPublisher capPublisher ) {
            _logger = logger;
            _cache = cache;
            _current = current;
            _dataContainer = dataContainer;
            _typeFinder = typeFinder;
            _typeResolve = typeResolve;
            _deviceService = deviceService;
            _dbContext = dbContext;
            _eventQueueService = eventQueueService;
            _capBus = capPublisher;
        }


        public async Task Execute( NullJobArgs args ) {
            if( !_isRuning ) {
                _isRuning = true;

                try {
                    var a = await _dbContext.ExecuteQuery<object>( $"DELETE TOP (10000) FROM EventQueue WITH (readpast) WHERE OutTime IS NOT Null and CreateTime < '{DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd")}';" );
                }
                catch( Exception ex ) {
                    _logger.LogError( "EventQueueDeleteExpiryJob Error : " + ex );
                }


                _isRuning = false;
            }

        }

    }
}
