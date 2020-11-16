using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Syinpo.Core.Data
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class {
        private readonly IDbContext _dbContext;
        private readonly DbContextFactory _dbContextFactory;
        private readonly IDatabaseHelper _databaseHelper;

        public GenericRepository( IDatabaseHelper databaseHelper, DbContextFactory dbContextFactory ) {
            _dbContextFactory = dbContextFactory;
            _databaseHelper = databaseHelper;
        }

        private string TryDecodeDbUpdateException( DbUpdateException ex ) {
            var sb = new StringBuilder();
            sb.AppendLine( $"DbUpdateException error details - {ex?.InnerException?.Message}" );

            foreach( var eve in ex.Entries ) {
                sb.AppendLine( $"Entity of type {eve.Entity.GetType().Name} in state {eve.State} could not be updated" );
            }

            return sb.ToString();
        }


        public async Task<TEntity> GetById( int id ) {
            if( _databaseHelper.AccorMainDb )
                return await _dbContextFactory.WriteDbContext.Set<TEntity>().FindAsync( id );

            return await _dbContextFactory.GetDbContext( false ).Set<TEntity>().FindAsync( id );
        }

        public async Task Create( TEntity entity ) {
            try {
                await _dbContextFactory.WriteDbContext.Set<TEntity>().AddAsync( entity );
                await _dbContextFactory.WriteDbContext.SaveChangesAsync();
            }
            catch( DbUpdateException ex ) {
                throw new Exception( TryDecodeDbUpdateException( ex ), ex );
            }
        }

        public async Task CreateRange(List<TEntity> entityList)
        {
            try
            {
                await _dbContextFactory.WriteDbContext.Set<TEntity>().AddRangeAsync(entityList);
                await _dbContextFactory.WriteDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(TryDecodeDbUpdateException(ex), ex);
            }
        }

        public async Task Update( TEntity entity ) {
            try
            {

                //try
                //{
                //    if (entity.GetType().Name == "DeviceProxy")
                //    {
                //        Type type = entity.GetType();
                //        IoC.Resolve<ILogger<string>>().LogError($" device update dbcontext : {type.GetProperty("DeviceUuid").GetValue(entity)}  PartnerSerialKeyId  [{type.GetProperty("PartnerSerialKeyId").GetValue(entity)}] ");
                //    }
                //}
                //catch (Exception)
                //{

                //    throw new Exception($"entity.GetType().Name :{entity.GetType().Name}");
                //}



                _dbContextFactory.WriteDbContext.Set<TEntity>().Update(entity);

                //if( entity.GetType().Name.Contains( "Device" ) ) {
                //    IoC.Resolve<ILogger<DeviceGroup>>().LogError( "device update dbcontext :" +
                //                                                  _dbContextFactory.WriteDbContext.GetType().Name +
                //     ( _dbContextFactory.WriteDbContext as DbContext ).Database.GetDbConnection().ConnectionString );
                //}


                await _dbContextFactory.WriteDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(TryDecodeDbUpdateException(ex), ex);
            }
        }

        public async Task UpdateRange(List<TEntity> entityList)
        {
            try
            {
                _dbContextFactory.WriteDbContext.Set<TEntity>().UpdateRange(entityList);
                await _dbContextFactory.WriteDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(TryDecodeDbUpdateException(ex), ex);
            }
        }

        public async Task Delete( TEntity entity ) {
            try {
                _dbContextFactory.WriteDbContext.Set<TEntity>().Remove( entity );
                await _dbContextFactory.WriteDbContext.SaveChangesAsync();
            }
            catch( DbUpdateException ex ) {
                throw new Exception( TryDecodeDbUpdateException( ex ), ex );
            }
        }

        public async Task DeleteRange(List<TEntity> entityList)
        {
            try
            {
                _dbContextFactory.WriteDbContext.Set<TEntity>().RemoveRange(entityList);
                await _dbContextFactory.WriteDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(TryDecodeDbUpdateException(ex), ex);
            }
        }

        public virtual IQueryable<TEntity> Table {
            get {
                if( _databaseHelper.AccorMainDb )
                    return WriteTable;

                //if( typeof( TEntity ).Name.EndsWith( "Device" ) ) {
                //    IoC.Resolve<ILogger<DeviceGroup>>().LogError( "ReadTable :" +
                //                                                  _dbContextFactory.GetDbContext( false ).GetType().Name +
                //                                                  ( _dbContextFactory.GetDbContext( false ) as DbContext )
                //                                                  .Database.GetDbConnection().ConnectionString );
                //}

                return AutoTable;
            }
        }

        public virtual IQueryable<TEntity> WriteTable {
            get {

                //if ( typeof( TEntity ).Name.EndsWith( "Device"))
                //{
                //    IoC.Resolve<ILogger<DeviceGroup>>().LogError("WriteTable :" +
                //                                                 _dbContextFactory.WriteDbContext.GetType().Name +
                //                                                 (_dbContextFactory.WriteDbContext as DbContext)
                //                                                 .Database.GetDbConnection().ConnectionString);
                //}
                if( _databaseHelper.NotCacheable ) {
                    return _dbContextFactory.WriteDbContext.Set<TEntity>().NotCacheable();
                }
                
                return _dbContextFactory.WriteDbContext.Set<TEntity>();
            }
        }

        public virtual IQueryable<TEntity> AutoTable {
            get {
                if( _databaseHelper.NotCacheable ) {
                    return _dbContextFactory.GetDbContext( false ).Set<TEntity>().NotCacheable();
                }

                return _dbContextFactory.GetDbContext( false ).Set<TEntity>();
            }
        }
    }
}


