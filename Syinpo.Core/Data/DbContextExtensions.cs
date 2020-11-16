using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using StackExchange.Redis;

namespace Syinpo.Core.Data {
    public static class DbContextExtensions {
        public static void UseTransaction( this IDbContext context, Action handler ) {
            context.Database.BeginTransaction();
            try {
                handler();
                context.Database.CommitTransaction();
            }
            catch( Exception ) {
                context.Database.RollbackTransaction();
                throw;
            }
        }


        public static async Task UseTransaction( this IDbContext context, Func<Task> handler ) {
            context.Database.BeginTransaction();
            try {
                await handler();
                context.Database.CommitTransaction();
            }
            catch( Exception ) {
                context.Database.RollbackTransaction();
                throw;
            }
        }
    }
}
