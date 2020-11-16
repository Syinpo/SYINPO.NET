using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Microsoft.Data.SqlClient;

namespace Syinpo.Core.Monitor.MsSqlModule {
    public class SqlParameterFactory : ParameterFactory<SqlParameter> {
        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <returns></returns>
        internal override SqlParameter CreateParameter() {
            return new SqlParameter();
        }
    }

    public abstract class ParameterFactory<T> where T : DbParameter {
        internal abstract T CreateParameter();

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <param name="theValue">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="direction">The direction of the parameter.</param>
        /// <returns></returns>
        public T CreateParameter( object theValue, string parameterName, ParameterDirection direction ) {
            ValidateParameter( parameterName );

            object valueToUse = theValue;

            if( theValue == null ) {
                valueToUse = DBNull.Value;
            }

            T parameter = CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = valueToUse;
            parameter.Direction = direction;

            return parameter;
        }

        // ReSharper disable once UnusedParameter.Local
        private static void ValidateParameter( string parameterName ) {
            if( string.IsNullOrWhiteSpace( parameterName ) ) {
                throw new ArgumentException( "ArgumentNullException", "parameterName" );
            }
        }
    }

    public static class StringExtensions {
        /// <summary>
        /// Replaces a single quote to a double single quote.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string ReplaceSingleQuote( this string sql ) {
            if( sql == null ) {
                throw new ArgumentNullException( "sql" );
            }
            return sql.Replace( "'", "''" );
        }
    }
}
