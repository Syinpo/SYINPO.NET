using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Syinpo.Core;
using Syinpo.Core.Monitor;
using Syinpo.Core.Monitor.MsSqlModule;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using OpenTelemetry.Trace.Configuration;
using Z.EntityFramework.Extensions;
using DbCommandInterceptor = Microsoft.EntityFrameworkCore.Diagnostics.DbCommandInterceptor;

namespace Syinpo.Unity.EfCore {
    public class HintCommandInterceptor : DbCommandInterceptor {
        //private readonly Tracer _tracer;
        //private readonly ILogger<HintCommandInterceptor> _logger;

        //public HintCommandInterceptor(Tracer tracer, ILogger<HintCommandInterceptor> logger)
        //{
        //    _tracer = tracer;
        //    _logger = logger;
        //}


        public override InterceptionResult<DbDataReader> ReaderExecuting(
            System.Data.Common.DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result ) {
            // 在此处操作命令文本等.
            //command.CommandText += " OPTION (OPTIMIZE FOR UNKNOWN)";
            try {
                var options = IoC.Resolve<IOptions<MonitorOptions>>()?.Value;
                if (options!=null&&options.UseMonitor)
                {
                    var _tracer = IoC.Resolve<Tracer>();
                    if (_tracer != null)
                    {
                        var traceId = _tracer.CurrentSpan.Context.TraceId.ToHexString();
                        if (traceId != "00000000000000000000000000000000")
                        {
                            var p = new List<SqlParameter>();
                            foreach (var commandParameter in command.Parameters)
                            {
                                p.Add((SqlParameter)commandParameter);
                            }

                            var sql = SqlDebugHelper.CreateExecutableSqlStatement(command.CommandText, p.ToArray());

                            using (_tracer.StartActiveSpan("sql-get-" + CommonHelper.NewSequentialGuid(), out var span))
                            {
                                span.SetAttribute(MonitorKeys.request_type,
                                    RequestTypeEnum.Sql.ToString().ToLowerInvariant());
                                span.SetAttribute(MonitorKeys.sql_raw, command.CommandText);
                                span.SetAttribute(MonitorKeys.sql_body, sql);

                                span.End();
                            }
                        }
                    }
                }
            }
            catch( Exception ex ) {
                var _logger = IoC.Resolve<ILogger<HintCommandInterceptor>>();
                _logger.LogError( "HintCommandInterceptor error:" + ex.Message );
            }

            return base.ReaderExecuting( command, eventData, result );
        }
    }

    public class NoLockInterceptor : DbCommandInterceptor {
        private static readonly Regex TableAliasRegex =
            new Regex( @"(?<tableAlias>AS \[[a-zA-Z]\w*\](?! WITH \(NOLOCK\)))",
                RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase );
        public override InterceptionResult<object> ScalarExecuting( DbCommand command, CommandEventData eventData, InterceptionResult<object> result ) {
            command.CommandText = TableAliasRegex.Replace(
                command.CommandText,
                "${tableAlias} WITH (NOLOCK)"
            );
            return base.ScalarExecuting( command, eventData, result );
        }
        public override Task<InterceptionResult<object>> ScalarExecutingAsync( DbCommand command, CommandEventData eventData, InterceptionResult<object> result,
            CancellationToken cancellationToken = new CancellationToken() ) {
            command.CommandText = TableAliasRegex.Replace(
                command.CommandText,
                "${tableAlias} WITH (NOLOCK)"
            );
            return base.ScalarExecutingAsync( command, eventData, result, cancellationToken );
        }
        public override InterceptionResult<DbDataReader> ReaderExecuting( DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result ) {
            command.CommandText = TableAliasRegex.Replace(
                command.CommandText,
                "${tableAlias} WITH (NOLOCK)"
            );
            return result;
        }
        public override Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync( DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = new CancellationToken() ) {
            command.CommandText = TableAliasRegex.Replace(
                command.CommandText,
                "${tableAlias} WITH (NOLOCK)"
            );
            return base.ReaderExecutingAsync( command, eventData, result, cancellationToken );
        }
    }

    public class CustomSqlServerQuerySqlGenerator : SqlServerQuerySqlGenerator {
        public CustomSqlServerQuerySqlGenerator( QuerySqlGeneratorDependencies dependencies )
            : base( dependencies ) { }
        protected override Expression VisitTable( TableExpression tableExpression ) {
            var result = base.VisitTable( tableExpression );
            Sql.Append( " WITH (NOLOCK)" );
            return result;
        }
    }


    public class CustomSqlServerQuerySqlGeneratorFactory : SqlServerQuerySqlGeneratorFactory {
        private readonly QuerySqlGeneratorDependencies _dependencies;
        public CustomSqlServerQuerySqlGeneratorFactory( QuerySqlGeneratorDependencies dependencies )
            : base( dependencies ) {
            _dependencies = dependencies;
        }
        public override QuerySqlGenerator Create() =>
            new CustomSqlServerQuerySqlGenerator( _dependencies );
    }
}
