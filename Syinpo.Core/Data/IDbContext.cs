using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Z.BulkOperations;

namespace Syinpo.Core.Data {
    public interface IDbContext {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        int SaveChanges();

        Task<int> SaveChangesAsync();

        Task<IEnumerable<TQuery>> ExecuteQuery<TQuery>( string sql ) where TQuery : class;

        Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade Database{ get; }

        //void BulkInsert2<TEntity>( IList<TEntity> data ) where TEntity : class;
        //void BulkUpdate2<TEntity>( IList<TEntity> data ) where TEntity : class;
        //void BulkDelete2<TEntity>( IList<TEntity> data ) where TEntity : class;
        void BulkMerge2<TEntity>( IList<TEntity> data, Action<BulkOperation<TEntity>> options = null ) where TEntity : class;
        void BulkSynchronize2<TEntity>(IList<TEntity> data, Action<BulkOperation<TEntity>> options = null) where TEntity : class;
    }
}
