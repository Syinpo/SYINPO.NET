using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.Jobs;
using Syinpo.BusinessLogic.Monitor;
using Syinpo.BusinessLogic.Notifications;
using Syinpo.BusinessLogic.Safety;
using Syinpo.BusinessLogic.SignalR.Online;
using Syinpo.Core;
using Syinpo.Core.Data;
using Syinpo.Core.Domain.Poco;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Core.Extensions;
using Syinpo.Core.Hangfire;
using Syinpo.Core.Helpers;
using Syinpo.Core.Mapper;
using Syinpo.Core.Monitor;
using Syinpo.Core.Monitor.PackModule;
using Syinpo.Core.Monitor.TracerModule;
using Syinpo.Unity.AspNetCore.Controllers;
using Syinpo.Unity.AspNetCore.Filters;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;

namespace Syinpo.Monitor.Api.Controllers {
    /// <summary>
    /// 监控API
    /// </summary>
    [SwaggerTag( "监控API" )]
    public class MonitorController : BaseApiController {
        private readonly IPackStore<MonitorEvent> _monitorService;
        private readonly IPackStore<TimeData> _monitorTimeDataManager;
        private readonly ILogger<MonitorController> _logger;
        private readonly IOptions<MonitorOptions> _settings;
        private readonly IOnlineManager _onlineManager;
        private readonly IPackStore<HttpLog> _httpLogStore;

        public MonitorController( IPackStore<HttpLog> httpLogStore,
            IPackStore<MonitorEvent> monitorService,
            IPackStore<TimeData> monitorTimeDataManager,
            ILogger<MonitorController> logger,
            IOptions<MonitorOptions> settings,
            IOnlineManager onlineManager ) {
            _monitorService = monitorService;
            _monitorTimeDataManager = monitorTimeDataManager;
            _logger = logger;
            _settings = settings;
            _onlineManager = onlineManager;
            _httpLogStore = httpLogStore;
        }

        /// <summary>
        /// Test
        /// </summary>
        /// <returns>返回Response</returns>
        /// <remarks>
        /// Test。
        /// </remarks>
        [HttpGet]
        [Route( "/api/batch/test" )]
        [SwaggerResponse( 0, "成功", typeof( Response<string> ) )]
        [SwaggerResponse( -1, "失败", typeof( ErrorObject ) )]
        public async Task<IActionResult> Test() {
            var a = _httpLogStore;
            await Task.CompletedTask;

            return Success();
        }

        /// <summary>
        /// MonitorEvent
        /// </summary>
        /// <param name="file">请求数据</param>
        /// <returns>返回Response</returns>
        /// <remarks>
        /// MonitorEvent。
        /// </remarks>
        [HttpPost]
        [Route( "/api/batch/events" )]
        [SwaggerResponse( 0, "成功", typeof( Response<string> ) )]
        [SwaggerResponse( -1, "失败", typeof( ErrorObject ) )]
        public async Task<IActionResult> CreateBatchEvents( [FromForm]IFormFile file ) {
            var files = Request.Form.Files;
            if( !files.Any() )
                return Error( propertyKey: "file", errorMessage: "没有文件" );

            await Execute( files, _settings.Value.ResponseStorePath );

            return Success();
        }

        /// <summary>
        /// TimeData
        /// </summary>
        /// <param name="file">请求数据</param>
        /// <returns>返回Response</returns>
        /// <remarks>
        /// TimeData
        /// </remarks>
        [HttpPost]
        [Route( "/api/batch/timedata" )]
        [SwaggerResponse( 0, "成功", typeof( Response<string> ) )]
        [SwaggerResponse( -1, "失败", typeof( ErrorObject ) )]
        public async Task<IActionResult> CreateBatchTimeData( [FromForm]IFormFile file ) {
            var files = Request.Form.Files;
            if( !files.Any() )
                return Error( propertyKey: "file", errorMessage: "没有文件" );

            await Execute( files, _settings.Value.TimeDataStorePath );

            return Success();
        }


        /// <summary>
        /// HttpLog
        /// </summary>
        /// <param name="file">请求数据</param>
        /// <returns>返回Response</returns>
        /// <remarks>
        /// HttpLog。
        /// </remarks>
        [HttpPost]
        [Route( "/api/batch/logs" )]
        [SwaggerResponse( 0, "成功", typeof( Response<string> ) )]
        [SwaggerResponse( -1, "失败", typeof( ErrorObject ) )]
        public async Task<IActionResult> CreateBatchLogs( [FromForm]IFormFile file ) {
            var files = Request.Form.Files;
            if( !files.Any() )
                return Error( propertyKey: "file", errorMessage: "没有文件" );

            await Execute( files, _settings.Value.RequestStorePath );

            return Success();
        }


        private async Task Execute( IFormFileCollection files, string findStorePath ) {
            var root = Path.Combine( CommonHelper.MapPath( findStorePath ), "zip" );
            if( !Directory.Exists( root ) ) {
                Directory.CreateDirectory( root );
            }

            foreach( var formFile in files ) {
                if( formFile.Length > 0 ) {
                    var originalFileName = formFile.FileName;
                    var fileName = $"{CommonHelper.NewSequentialGuid()}{Path.GetExtension( originalFileName )}";
                    var path = Path.Combine( root, originalFileName );

                    using( var stream = System.IO.File.Create( path ) ) {
                        await formFile.CopyToAsync( stream );
                    }
                }
            }
        }
    }
}