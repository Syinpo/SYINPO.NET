using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace Syinpo.Core.Monitor.MsSqlModule {
    public static class SqlDebugHelper {
        /// <summary>
        /// The object that creates the SQL statements. You can override this class and implement your own if you're having issues with the generated SQL.
        /// </summary>
        public static SqlGenerator SqlGenerator = new SqlGenerator();

        /// <summary>
        /// Creates a SQL-string with the parameter declaration and the sql statement so it can be executed in Sql Server Management studio.
        /// </summary>
        /// <param name="sql">The sql-query</param>
        /// <param name="parameters">The SqlParameters used for the sql-statement</param>
        /// <returns></returns>
        public static string CreateExecutableSqlStatement( string sql, params SqlParameter[] parameters ) {
            if( sql == null ) {
                throw new ArgumentNullException( "sql" );
            }
            return CreateExecutableSqlStatement( sql, SqlGenerator.GetCommandType( sql ), parameters );
        }

        /// <summary>
        /// Creates a SQL-string with the parameter declaration and the sql statement so it can be executed in Sql Server Management studio.
        /// </summary>
        /// <param name="sql">The sql-query</param>
        /// <param name="parameters">The SqlParameters used for the sql-statement</param>
        /// <returns></returns>
        public static string CreateExecutableSqlStatement( string sql, IList<SqlParameter> parameters ) {
            var sqlParameters = SqlGenerator.ConvertToParams( parameters );
            return CreateExecutableSqlStatement( sql, sqlParameters );
        }

        /// <summary>
        /// Creates a SQL-string with the parameter declaration and the sql statement so it can be executed in Sql Server Management studio.
        /// </summary>
        /// <param name="sql">The sql-query</param>
        /// <param name="commandType">The <see cref="CommandType"/>. Only StoredProcedure and Text are supported.</param>
        /// <param name="parameters">The SqlParameters used for the sql-statement</param>
        /// <returns></returns>
        public static string CreateExecutableSqlStatement( string sql, CommandType commandType, IList<SqlParameter> parameters ) {
            var param = SqlGenerator.ConvertToParams( parameters );
            return SqlGenerator.CreateExecutableSqlStatement( sql, commandType, param );
        }
    }
}
