using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Syinpo.Core.Helpers
{
    public static class AsyncHelper {
        /// <summary>
        /// Runs a async method synchronously.
        /// </summary>
        /// <param name="func">A function that returns a result</param>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <returns>Result of the async operation</returns>
        public static TResult RunSync<TResult>( Func<Task<TResult>> func ) {
            return func.Invoke().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Runs a async method synchronously.
        /// </summary>
        /// <param name="action">An async action</param>
        public static void RunSync( Func<Task> action ) {
            action.Invoke().ConfigureAwait( false ).GetAwaiter().GetResult();
        }
    }
}
