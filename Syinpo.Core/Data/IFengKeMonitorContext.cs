using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Syinpo.Core.Domain.MonitorPoco;
using Syinpo.Core.Domain.Poco;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Z.BulkOperations;

namespace Syinpo.Core.Data {
    public partial class SyinpoMonitorContext : DbContext, IDbContext {

        public SyinpoMonitorContext( DbContextOptions<SyinpoMonitorContext> options ) : base( options ) {
        }

        protected override void OnModelCreating( ModelBuilder modelBuilder ) {
            modelBuilder.Entity<RequestLog>( entity =>
            {
                entity.ToTable( "RequestLog" );
            } );
            modelBuilder.Entity<ResponseSnap>( entity =>
            {
                entity.ToTable( "ResponseSnap" );
            } );
            modelBuilder.Entity<TrafficStatist>( entity =>
            {
                entity.ToTable( "TrafficStatist" );
            } );
            modelBuilder.Entity<SqlSnap>( entity =>
            {
                entity.ToTable( "SqlSnap" );
            } );
            modelBuilder.Entity<ExceptionSnap>( entity =>
            {
                entity.ToTable( "ExceptionSnap" );
            } );
            modelBuilder.Entity<TrackSnap>( entity =>
            {
                entity.ToTable( "TrackSnap" );
            } );

            OnModelCreatingPartial( modelBuilder );
        }

        partial void OnModelCreatingPartial( ModelBuilder modelBuilder );


       public Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade Database {
            get {
                return this.Database;
            }
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class {
            return base.Set<TEntity>();
        }

        public async Task<int> SaveChangesAsync() {
            return await base.SaveChangesAsync();
        }

        public async Task<IEnumerable<TQuery>> ExecuteQuery<TQuery>( string sql ) where TQuery : class {
            using( var conn = new SqlConnection( this.Database.GetDbConnection().ConnectionString ) ) {
                return await conn.QueryAsync<TQuery>( sql, commandType: CommandType.Text );
            }
        }

        public void BulkInsert2<TEntity>( IList<TEntity> data ) where TEntity : class {
            this.BulkInsert( data, options =>
            {
                options.SqlBulkCopyOptions = (int)System.Data.SqlClient.SqlBulkCopyOptions.FireTriggers;
            } );
        }

        public void BulkUpdate2<TEntity>( IList<TEntity> data ) where TEntity : class {
            this.BulkUpdate( data, options =>
            {
                options.SqlBulkCopyOptions = (int)System.Data.SqlClient.SqlBulkCopyOptions.FireTriggers;
            } );
        }

        public void BulkDelete2<TEntity>( IList<TEntity> data ) where TEntity : class {
            this.BulkDelete( data );
        }

        public void BulkMerge2<TEntity>( IList<TEntity> data, Action<BulkOperation<TEntity>> options = null ) where TEntity : class {
            if( options == null )
                this.BulkMerge( data );
            else {
                this.BulkMerge( data, options );
            }
        }

        public void BulkSynchronize2<TEntity>( IList<TEntity> data, Action<BulkOperation<TEntity>> options = null ) where TEntity : class {
            if( options == null )
                this.BulkSynchronize( data );
            else {
                this.BulkSynchronize( data, options );
            }
        }
    }
}
