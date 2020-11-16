using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syinpo.Core;
using Syinpo.Core.Data;
using Syinpo.Core.Domain.MonitorPoco;
using Syinpo.Core.Helpers;
using Syinpo.Core.Mapper;
using Syinpo.Core.Monitor;
using Syinpo.Core.Monitor.PackModule;
using Syinpo.Core.Monitor.TracerModule;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Syinpo.Monitor.Response
{
    class Program
    {
        static async Task Main(string[] args)
        {
            new BootStart().Start();
            var logger = IoC.Resolve<ILogger<Program>>();
            var options = IoC.Resolve<IOptions<MonitorOptions>>()?.Value;

            // app start
            logger.LogInformation("Syinpo.Monitor.Response 启动");

            while (true)
            {
                try
                {
                    await UnZip(logger, options.FindResponseLogPath);

                    await Log(logger, options);
                }
                catch (Exception ex)
                {
                    logger.LogError("Syinpo.Monitor.Response 错误：" + ex.Message);
                }

                System.Threading.Thread.Sleep(1000 * 10);
            }
        }

        private static async Task UnZip(ILogger<Program> logger, string path)
        {
            logger.LogInformation("开始批量解压目录文件：" + path);

            var path1 = Path.Combine(path, "zip");
            var path2 = Path.Combine(path, "extract");

            MonitorZipFileHelper.UnFromDirectory(path1, path2);

            logger.LogInformation("解压完成。" + DateTime.Now);

            await Task.CompletedTask;
        }

        private static async Task Log(ILogger<Program> logger, MonitorOptions options)
        {
            logger.LogInformation("开始批量处理Log文件");

            var files = ListFiles(Path.Combine(options.FindResponseLogPath, "extract"));
            foreach (var file in files)
            {
                logger.LogInformation("正在处理Log文件：" + file.Name);

                var path = CommonHelper.MapPath(file.FullName);
                var data = MessagePackHelper.ToObject<List<MonitorEvent>>(path);

                var resultResponse = new List<ResponseSnap>();
                var resultSql = new List<SqlSnap>();
                var resultError = new List<ExceptionSnap>();
                var resultTrace = new List<TrackSnap>();
                foreach (var item in data)
                {
                    var traceId = GetValue<string>(item.Data, MonitorKeys.trace_id);

                    if (item.EventType == RequestTypeEnum.Http.ToString().ToLowerInvariant())
                    {
                        var snap = PreResponse(item, traceId);
                        resultResponse.Add(snap);
                    }
                    else if (item.EventType == RequestTypeEnum.Signalr.ToString().ToLowerInvariant())
                    {
                        var snap = PreResponse(item, traceId);
                        resultResponse.Add(snap);
                    }
                    else if (item.EventType == RequestTypeEnum.Sql.ToString().ToLowerInvariant())
                    {
                        var snap = PreSql(item, traceId);
                        resultSql.Add(snap);
                    }
                    else
                    {
                        var snap = PreTrace(item, traceId);
                        if (snap != null)
                        {
                            resultTrace.Add(snap);
                        }
                        else
                        {
                            logger.LogWarning("事件{0}，得EventType为{1}，不正确", traceId, item.EventType);
                            continue;
                        }
                    }


                    if (item.Data.ContainsKey(MonitorKeys.request_error))
                    {
                        var source = GetValue<string>(item.Data, MonitorKeys.request_error);
                        var errormessage = GetValue<string>(item.Data, MonitorKeys.request_errordetail);

                        var snap = new ExceptionSnap
                        {
                            TraceId = traceId,
                            ErrorSource = source,
                            ErrorDetail = errormessage,
                            CreateTime = DateTime.Now
                        };

                        resultError.Add(snap);
                    }
                }

                var dbContext = IoC.Resolve<IDbContext>("monitor_dbcontext");
                if (resultResponse.Any())
                {
                    await IoC.Resolve<IGenericRepository<ResponseSnap>>().CreateRange(resultResponse);
                }
                if (resultSql.Any())
                {
                    await IoC.Resolve<IGenericRepository<SqlSnap>>().CreateRange(resultSql);
                }
                if (resultError.Any())
                {
                    await IoC.Resolve<IGenericRepository<ExceptionSnap>>().CreateRange(resultError);
                }
                if (resultTrace.Any())
                {
                    await IoC.Resolve<IGenericRepository<TrackSnap>>().CreateRange(resultTrace);
                }

                File.Delete(file.FullName);
            }

            logger.LogInformation("结束这批处理Log文件，共:" + files.Count());

            await Task.CompletedTask;
        }




        public static ResponseSnap PreResponse(MonitorEvent item, string traceId)
        {
            var success = GetValue<bool>(item.Data, MonitorKeys.response_success);
            var body = GetValue<string>(item.Data, MonitorKeys.response_body);

            if (false)
            {
                if (success && item.EventType == RequestTypeEnum.Http.ToString().ToLowerInvariant())
                {
                    success = TryParseJson(body, out JObject jObject);
                    if (success)
                    {
                        success = jObject.TryGetValue("code", out JToken jToken);
                        if (success)
                        {
                            success = jToken != null && jToken.ToString() == "0";
                        }
                    }
                }
                else if (success && item.EventType == RequestTypeEnum.Signalr.ToString().ToLowerInvariant())
                {
                    success = TryParseJson(body, out JObject jObject);
                    if (success)
                    {
                        success = jObject.TryGetValue("success", out JToken jToken);
                        if (success)
                        {
                            success = jToken != null && jToken.ToString().ToLowerInvariant() == "true";
                        }
                    }
                }
            }

            // response
            var contentlength = body == null ? 0 : Encoding.Default.GetByteCount(body);
            var elapsed = GetValue<long>(item.Data, MonitorKeys.response_elapsed);
            var statusCode = GetValue<int?>(item.Data, MonitorKeys.response_statuscode);
            var snap = new ResponseSnap
            {
                TraceId = traceId,
                StatusCode = statusCode,
                Success = success,
                ContentLength = contentlength,
                ResponseBody = body,
                Elapsed = elapsed,
                CreateTime = DateTime.Now,
            };

            return snap;
        }

        public static SqlSnap PreSql(MonitorEvent item, string traceId)
        {
            var sqlRaw = GetValue<string>(item.Data, MonitorKeys.sql_raw);
            var sqlBody = GetValue<string>(item.Data, MonitorKeys.sql_body);
            var snap = new SqlSnap
            {
                TraceId = traceId,
                SqlRaw = sqlRaw,
                SqlBody = sqlBody,
                Elapsed = 0,
                CreateTime = DateTime.Now,
            };

            return snap;
        }

        public static TrackSnap PreTrace(MonitorEvent item, string traceId)
        {
            var traceName = GetValue<string>(item.Data, MonitorKeys.service_name);
            if (string.IsNullOrEmpty(traceName))
                return null;

            var elapsed = GetValue<long>(item.Data, MonitorKeys.response_elapsed);
            var snap = new TrackSnap
            {
                TraceId = traceId,
                TraceName = traceName,
                TraceData = item.Data.ToJson(),
                Elapsed = elapsed,
                CreateTime = DateTime.Now,
            };

            return snap;
        }

        public static T GetValue<T>(Dictionary<string, object> source, string key)
        {
            var v1 = source.TryGetValue(key, out object val);

            if (v1)
            {
                return CommonHelper.ConvertTo<T>(val);
            }

            return default(T);
        }

        public static IEnumerable<FileInfo> ListFiles(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new ArgumentException("目录不能存在： " + path);
            }

            return new DirectoryInfo(path)
                .GetFiles("*", SearchOption.AllDirectories)
                .ToList();
        }

        public static bool TryParseJson(string obj, out JObject result)
        {
            try
            {
                result = JObject.Parse(obj);
                return true;
            }
            catch (Exception)
            {
                result = default(JObject);
                return false;
            }
        }
    }
}
