using Syinpo.Core;
using Syinpo.Core.Caches;
using Syinpo.Core.Extensions;
using Syinpo.Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Syinpo.Unity.AspNetCore.Filters
{
    public class DuplicateRequestFilter : ActionFilterAttribute
    {
        private readonly IMd5Cache _md5Cache;
        private readonly ILogger<DuplicateRequestFilter> _logger;
        private readonly string _argumentName;
        private readonly int _cycleTime;

        public DuplicateRequestFilter(string argumentName, int cycleTime = 60)
        {
            _md5Cache = IoC.Resolve<IMd5Cache>("DeviceApi");
            _logger = IoC.Resolve<ILogger<DuplicateRequestFilter>>();
            _argumentName = argumentName;
            _cycleTime = cycleTime < 60 ? 60 : cycleTime; //最少60秒
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;
            if (context.ModelState.IsValid && context.ActionArguments.Keys.Contains(_argumentName) && context.ActionArguments.TryGetValue(_argumentName, out object dto))
            {
                var body = JsonHelper.ToJson(dto);
                var ip = CommonHelper.GetClientIpAddress() ?? "null";
                //根据设置周期时间，在一定时间(秒)内不允许重复
                var serialNumber = Convert.ToInt32(Math.Floor(DateTime.Now.Subtract(DateTime.Today).TotalSeconds / _cycleTime));
                var signature = $"IP:{ip}, Path:{request.Path}({serialNumber})";
                var data = $"{signature}, Body:{body}";
                var md5 = data.CreateMD5();
                //var logData = data.Length > 4096 ? data.Substring(0, 4096) : data;
                if (!_md5Cache.IsEmpty && _md5Cache.Contains(md5))
                {
                    //while (data.Length > 10240)
                    //{
                    //    logData = data.Substring(0, 10240);
                    //    data = data.Substring(10240);
                    //    _logger.LogError($"重复请求(MD5:{md5}, ContentLength:{body.Length}) @@Data>>{logData}");
                    //}
                    //_logger.LogError($"重复请求(MD5:{md5}, ContentLength:{body.Length}) @@Data>>{data}");

                    //_logger.LogError($"重复请求(MD5:{md5}, ContentLength:{body.Length}) @@Data>>{logData}");

                    context.Result = new ContentResult()
                    {
                        StatusCode = StatusCodes.Status429TooManyRequests,
                        Content = "Duplicate Request"
                    };

                    return;
                }
                else
                {
                    _md5Cache.Enqueue(md5);

                    //while (data.Length > 10240)
                    //{
                    //    logData = data.Substring(0, 10240);
                    //    data = data.Substring(10240);
                    //    _logger.LogError($"正常请求(MD5:{md5}, ContentLength:{body.Length}) @@Data>>{logData}");
                    //}
                    //_logger.LogError($"正常请求(MD5:{md5}, ContentLength:{body.Length}) @@Data>>{data}");

                    //_logger.LogError($"正常请求(MD5:{md5}, ContentLength:{body.Length}) @@Data>>{logData}");
                }
            }

            await next();
        }

    }
}
