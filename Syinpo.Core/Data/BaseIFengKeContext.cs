using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Syinpo.Core.Domain.Poco;
using Microsoft.EntityFrameworkCore;
using Z.BulkOperations;

namespace Syinpo.Core.Data {
    public partial class BaseSyinpoContext<T> : DbContext, IDbContext where T : DbContext {
        public BaseSyinpoContext ( DbContextOptions<T>  options ):base( options) {
        }

        protected override void OnModelCreating( ModelBuilder modelBuilder ) {
            modelBuilder.Entity<Device>( entity => {
                entity.ToTable( "Device" );

                entity.Property( e => e.Id ).HasComment( "主键" );

                entity.Property( e => e.Approved ).HasComment( "批准" );

                entity.Property( e => e.AssistantVersion )
                    .HasMaxLength( 1000 )
                    .HasComment( "手机助手版本号" );

                entity.Property( e => e.Brand )
                    .HasMaxLength( 1000 )
                    .HasComment( "手机品牌" );

                entity.Property( e => e.CreateTime )
                    .HasColumnType( "datetime" )
                    .HasComment( "创建时间" );

                entity.Property( e => e.Deleted ).HasComment( "删除" );

                entity.Property( e => e.DeviceUuid )
                    .IsRequired()
                    .HasMaxLength( 1000 )
                    .HasComment( "设备IMIE" );

                entity.Property( e => e.IsOnline ).HasComment( "设备是否在线" );

                entity.Property( e => e.LastSyncContactTime )
                    .HasColumnType( "datetime" )
                    .HasComment( "最后同步手机联系人的时间" );

                entity.Property( e => e.Latitude )
                    .HasColumnType( "decimal(18, 6)" )
                    .HasComment( "纬度" );

                entity.Property( e => e.ListType ).HasComment( "所处名单 （0 无，1 在黑名单，2 在白名单）" );

                entity.Property( e => e.Longitude )
                    .HasColumnType( "decimal(18, 6)" )
                    .HasComment( "经度" );

                entity.Property( e => e.Memo )
                    .HasMaxLength( 500 )
                    .HasComment( "备注" );

                entity.Property( e => e.Mobile )
                    .HasMaxLength( 1000 )
                    .HasComment( "手机号码" );

                entity.Property( e => e.Model )
                    .HasMaxLength( 1000 )
                    .HasComment( "手机型号" );

                entity.Property( e => e.Os )
                    .HasMaxLength( 1000 )
                    .HasComment( "手机操作系统" );

                entity.Property( e => e.OsVersion )
                    .HasMaxLength( 1000 )
                    .HasComment( "手机操作系统版本号" );

                entity.Property( e => e.TrackingId )
                    .IsRequired()
                    .HasMaxLength( 50 )
                    .HasComment( "对外跟踪的设备Id" );

                entity.Property( e => e.UpdateTime )
                    .HasColumnType( "datetime" )
                    .HasComment( "更新时间" );

                entity.Property( e => e.WeiXinVersion )
                    .HasMaxLength( 1000 )
                    .HasComment( "手机微信版本号" );
            } );

            modelBuilder.Entity<DeviceSms>( entity => {
                entity.ToTable( "DeviceSms" );

                entity.HasIndex( e => e.CreateTime );

                entity.HasIndex( e => e.DeviceId );

                entity.HasIndex( e => e.FromPhone );

                entity.HasIndex( e => e.SmsCreateTime );

                entity.HasIndex( e => e.ToPhone );

                entity.Property( e => e.Id ).HasComment( "主键" );

                entity.Property( e => e.Content )
                    .IsRequired()
                    .HasComment( "短信内容" );

                entity.Property( e => e.CreateTime )
                    .HasColumnType( "datetime" )
                    .HasComment( "创建时间" );

                entity.Property( e => e.DeviceId ).HasComment( "设备主键" );

                entity.Property( e => e.FromPhone )
                    .IsRequired()
                    .HasMaxLength( 50 )
                    .HasComment( "从哪个号码" );

                entity.Property( e => e.SensitiveWord )
                    .HasMaxLength( 1000 )
                    .HasComment( "敏感词" );

                entity.Property( e => e.Sent ).HasComment( "是否发送，否则接收" );

                entity.Property( e => e.SmsCreateTime )
                    .HasColumnType( "datetime" )
                    .HasComment( "设备短信创建时间" );

                entity.Property( e => e.SmsReceiveTime )
                    .HasColumnType( "datetime" )
                    .HasComment( "设备短信收到时间" );

                entity.Property( e => e.ToPhone )
                    .IsRequired()
                    .HasMaxLength( 50 )
                    .HasComment( "到哪个号码" );

                entity.HasOne( d => d.Device )
                    .WithMany( p => p.DeviceSms )
                    .HasForeignKey( d => d.DeviceId )
                    .OnDelete( DeleteBehavior.ClientSetNull )
                    .HasConstraintName( "FK_DeviceSms_Device" );
            } );

            modelBuilder.Entity<EventQueue>( entity => {
                entity.ToTable( "EventQueue" );

                entity.Property( e => e.Id ).HasComment( "主键" );

                entity.Property( e => e.Content )
                    .IsRequired()
                    .HasComment( "数据" );

                entity.Property( e => e.CreateTime )
                    .HasColumnType( "datetime" )
                    .HasComment( "创建时间" );

                entity.Property( e => e.Heads )
                    .IsRequired()
                    .HasComment( "头部信息" );

                entity.Property( e => e.Note ).HasComment( "备注" );

                entity.Property( e => e.OutTime )
                    .HasColumnType( "datetime" )
                    .HasComment( "成功时间" );

                entity.Property( e => e.Retry ).HasComment( "重试次数" );

                entity.Property( e => e.RouteName )
                    .IsRequired()
                    .HasMaxLength( 400 )
                    .HasComment( "事件名称" );

                entity.Property( e => e.Version )
                    .IsRequired()
                    .HasMaxLength( 200 )
                    .HasComment( "隔离号" );
            } );

            modelBuilder.Entity<User>( entity => {
                entity.ToTable( "User" );

                entity.Property( e => e.Id ).HasComment( "主键" );

                entity.Property( e => e.Approved ).HasComment( "批准" );

                entity.Property( e => e.CreateTime )
                    .HasColumnType( "datetime" )
                    .HasComment( "创建时间" );

                entity.Property( e => e.Deleted ).HasComment( "删除" );

                entity.Property( e => e.DisplayName )
                    .HasMaxLength( 1000 )
                    .HasComment( "显示名称" );

                entity.Property( e => e.Email )
                    .HasMaxLength( 1000 )
                    .HasComment( "电子邮件" );

                entity.Property( e => e.IsAdmin ).HasComment( "是否超级管理员" );

                entity.Property( e => e.IsHttpUse ).HasComment( "对外用户" );

                entity.Property( e => e.LastLoginDate )
                    .HasColumnType( "datetime" )
                    .HasComment( "最后登录时间" );

                entity.Property( e => e.Mobile )
                    .HasMaxLength( 1000 )
                    .HasComment( "手机号码" );

                entity.Property( e => e.ParentUserId ).HasComment( "父用户主键" );

                entity.Property( e => e.PartnerId ).HasComment( "合作伙伴主键" );

                entity.Property( e => e.Password )
                    .IsRequired()
                    .HasMaxLength( 1000 )
                    .HasComment( "密码" );

                entity.Property( e => e.RealName )
                    .HasMaxLength( 1000 )
                    .HasComment( "真实姓名" );

                entity.Property( e => e.UpdateTime )
                    .HasColumnType( "datetime" )
                    .HasComment( "更新时间" );

                entity.Property( e => e.UserGuid ).HasComment( "对外使用主键" );

                entity.Property( e => e.Username )
                    .IsRequired()
                    .HasMaxLength( 1000 )
                    .HasComment( "用户名" );
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

        public virtual async Task<int> SaveChangesAsync() {
            return await base.SaveChangesAsync();
        }

        public async Task<IEnumerable<TQuery>> ExecuteQuery<TQuery>( string sql ) where TQuery : class
        {
            var str = (this as DbContext).Database.GetDbConnection().ConnectionString;
            using( var conn = new SqlConnection( str ) ) {
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
