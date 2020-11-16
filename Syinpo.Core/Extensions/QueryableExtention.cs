using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Syinpo.Core.Extensions {
    public static class QueryableExtention {
        public static IQueryable<TEntity> WhereIF<TEntity>( this IQueryable<TEntity> query, bool condition, Expression<Func<TEntity, bool>> predicate ) {
            return condition ? query.Where( predicate ) : query;
        }

        /// <summary>
        /// 获取本次查询SQL语句
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string ToSql<TEntity>( this IQueryable<TEntity> query ) {
            var enumerator = query.Provider.Execute<IEnumerable<TEntity>>( query.Expression ).GetEnumerator();
            var enumeratorType = enumerator.GetType();
            var selectFieldInfo = enumeratorType.GetField( "_selectExpression", BindingFlags.NonPublic | BindingFlags.Instance ) ?? throw new InvalidOperationException( $"cannot find field _selectExpression on type {enumeratorType.Name}" );
            var sqlGeneratorFieldInfo = enumeratorType.GetField( "_querySqlGeneratorFactory", BindingFlags.NonPublic | BindingFlags.Instance ) ?? throw new InvalidOperationException( $"cannot find field _querySqlGeneratorFactory on type {enumeratorType.Name}" );
            var selectExpression = selectFieldInfo.GetValue( enumerator ) as SelectExpression ?? throw new InvalidOperationException( $"could not get SelectExpression" );
            var factory = sqlGeneratorFieldInfo.GetValue( enumerator ) as IQuerySqlGeneratorFactory ?? throw new InvalidOperationException( $"could not get IQuerySqlGeneratorFactory" );
            var sqlGenerator = factory.Create();
            var command = sqlGenerator.GetCommand( selectExpression );
            var sql = command.CommandText;

            var builder = new StringBuilder();
            foreach( var parameter in command.Parameters ) {
                builder
                    .Append( "-- " )
                    .Append( parameter.InvariantName )
                    .Append( "='" )
                    .Append( parameter.ToString() )
                    .AppendLine( "'" );
            }

            return builder.Append( sql ).ToString();

            return sql;
        }
    }
}
