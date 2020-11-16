using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Syinpo.Core.Monitor;
using Microsoft.Extensions.Options;

namespace Syinpo.Core.Data {
    public class DbContextFactory {
        private readonly SqlBusOptions _settings;

        public DbContextFactory( IDbContext readDbContext, IDbContext writeDbContext, SqlBusOptions settings  ) {
            this.ReadDbContext = readDbContext;
            this.WriteDbContext = writeDbContext;
            this._settings = settings;
        }

        /// <summary>
        /// 是否使用事务
        /// </summary>
        public bool IsTransaction {
            get; set;
        }

        public IDbContext ReadDbContext {
            get; set;
        }

        public IDbContext WriteDbContext {
            get; set;
        }

        public void UseTransaction( Action handler ) {
            IsTransaction = true;

            {
                handler();
                return;
            }

            if( !_settings.UseReadWriteSpit ) {
                handler();
                return;
            }

            DbContext session = GetDbContext( true ) as DbContext;

            session.Database.BeginTransaction( System.Data.IsolationLevel.ReadUncommitted );
            try {
                handler();
                session.Database.CommitTransaction();
            }
            catch( Exception ) {
                session.Database.RollbackTransaction();
                throw;
            }
        }

        public async Task<T> UseTransaction<T>( Func<Task<T>> handler ) {
            IsTransaction = true;

            {
                var result = await handler();
                return result;
            }

            if( !_settings.UseReadWriteSpit ) {
                var result = await handler();
                return result;
            }


            DbContext session = GetDbContext( true ) as DbContext;

            session.Database.BeginTransaction( System.Data.IsolationLevel.ReadUncommitted );
            try {
                var result = await handler();
                session.Database.CommitTransaction();
                return result;
            }
            catch( Exception ) {
                session.Database.RollbackTransaction();
                throw;
            }


        }

        public void UseMainDb( Action handler ) {
            IsTransaction = true;

            handler();
        }

        public IDbContext GetDbContext( bool isWrite = true ) {
            if( IsTransaction )
                return WriteDbContext;

            if( isWrite )
                return WriteDbContext;

            return ReadDbContext;
        }
    }
}
