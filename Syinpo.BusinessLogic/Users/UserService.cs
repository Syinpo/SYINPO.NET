using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Caches;
using Syinpo.Core;
using Syinpo.Core.Caches;
using Syinpo.Core.Data;
using Syinpo.Core.Domain.Poco;
using Syinpo.Core.Helpers;
using Syinpo.Model.Request.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Syinpo.BusinessLogic.Users {
    public partial class UserService : IUserService {
        #region 字段

        private readonly IDbContext _dbContext;
        private readonly IGenericRepository<User> _userRepository;
        private readonly ICacheEvent _cacheEvent;
        private readonly ICache _cache;

        #endregion

        #region 构造函数

        public UserService(
            IDbContext dbContext,
            IGenericRepository<User> userRepository,
            IMemoryCache memoryCache,
            ICache cache,
            ICacheEvent cacheEvent ) {
            _dbContext = dbContext;
            _userRepository = userRepository;
            _cacheEvent = cacheEvent;
            _cache = cache;
        }

        #endregion

        #region 方法

        #region Users

        public async Task<PageList<User>> SearchUsers( DateTime? createTimeStart = null, DateTime? createTimeEnd = null, int partnerId = 0,
            int[] userRoleIds = null, int[] deptIds = null, string keywords = null, int pageIndex = 0, int pageSize = 10000 ) {
            var query = _userRepository.Table;
            if( createTimeStart.HasValue && createTimeStart.Value.Year > 1 )
                query = query.Where( c => createTimeStart.Value <= c.CreateTime );
            if( createTimeEnd.HasValue && createTimeEnd.Value.Year > 1 )
                query = query.Where( c => createTimeEnd.Value >= c.CreateTime );

            if( partnerId > 0 )
                query = query.Where( c => partnerId == c.PartnerId );

            if( !string.IsNullOrEmpty( keywords ) )
                query = query.Where( c => c.Username.Contains( keywords ) || c.Mobile.Contains( keywords ) || c.RealName.Contains( keywords ) || c.Email.Contains( keywords ) );


            query = query.Where( c => !c.Deleted );

            //隐藏用于http调用的默认用户
            query = query.Where( a => !a.IsHttpUse );


            query = query.OrderByDescending( c => c.CreateTime );

            var users = new PageList<User>( query, pageIndex, pageSize );

            return await Task.FromResult( users );
        }


        public async Task DeleteUser( User user ) {
            if( user == null )
                throw new ArgumentNullException( nameof( user ) );

            user.Username = user.Username + "-del-" + CommonHelper.NewSequentialGuid();
            user.Deleted = true;
            user.UpdateTime = DateTime.Now;
            await UpdateUser( user );
        }

        public async Task<User> GetUserById( int userId ) {
            if( userId == 0 )
                return null;

            return await _userRepository.GetById( userId );
        }

        public async Task<List<User>> GetUsersByIds( int[] userIds ) {
            if( userIds == null || userIds.Length == 0 )
                return new List<User>();

            var query = from c in _userRepository.Table
                        where userIds.Contains( c.Id ) && !c.Deleted
                        select c;
            var users = query.ToList();

            return await Task.FromResult( users );
        }

        public async Task<User> GetUserByGuid( Guid userGuid ) {
            if( userGuid == Guid.Empty )
                return null;

            var query = from c in _userRepository.Table
                        where c.UserGuid == userGuid
                        orderby c.Id
                        select c;
            var user = query.FirstOrDefault();
            return await Task.FromResult( user );
        }

        public async Task<User> GetUserByUsername( string username ) {
            if( string.IsNullOrWhiteSpace( username ) )
                return null;

            var user = await _userRepository.Table.Where( x => !x.Deleted && x.Username == username ).FirstOrDefaultAsync();
            return user;
        }

        public async Task InsertUser( User user ) {
            if( user == null )
                throw new ArgumentNullException( nameof( user ) );

            await _userRepository.Create( user );
            _cacheEvent.Publish( user, ChangeType.Insert );
        }

        public async Task UpdateUser( User user ) {
            if( user == null )
                throw new ArgumentNullException( nameof( user ) );

            await _userRepository.Update( user );
            _cacheEvent.Publish( user, ChangeType.Update );
        }


        #endregion

        #region UserManager

        public async Task<bool> CheckPassword( User user, string clearTextPassword ) {
            if( user == null || string.IsNullOrEmpty( clearTextPassword ) || string.IsNullOrEmpty( user.Password ) )
                return false;

            var encryptPassword = EncryptionHelper.EncryptText( clearTextPassword );

            return await Task.FromResult( user.Password.Equals( encryptPassword ) );
        }

        public async Task<(bool success, string errorMessage)> ValidateUser( string username, string clearTextPassword ) {

            var user = await GetUserByUsername( username );

            if( user == null )
                return (false, "用户不存在");
            if( user.Deleted )
                return (false, "用户已刪除");
            if( !user.Approved )
                return (false, "用户未批准使用");
            if( !await CheckPassword( user, clearTextPassword ) )
                return (false, "密码错误");

            return (true, "");
        }

        public async Task<(bool success, string errorMessage)> RegisterUser( UserRegisterRequest request ) {
            if( request == null )
                throw new ArgumentNullException( nameof( request ) );

            if( string.IsNullOrWhiteSpace( request.Password ) ) {
                return (false, "密码不能为空");
            }

            if( string.IsNullOrEmpty( request.Username ) ) {
                return (false, "用户名不能为空");
            }

            if( await GetUserByUsername( request.Username ) != null ) {
                return (false, "用户已存在");
            }

            var user = new User { };

            user.Username = request.Username;
            user.Email = request.Email;
            user.Password = EncryptionHelper.EncryptText( request.Password );
            user.Mobile = request.Mobile;
            user.Approved = request.Approved;


            await InsertUser( user );

            //var userDto = user.MapTo<UserDto>();
            //_memoryCache.Set<string>($"{Global.CacheKey.User}:{userDto.Username}", JsonHelper.ToJson(userDto), options);

            _cacheEvent.Publish( user, ChangeType.Insert );

            return (true, "用户注册成功");
        }

        public async Task<(bool success, string errorMessage)> ChangePassword( string username, string newPassword ) {
            if( string.IsNullOrWhiteSpace( username ) ) {
                return (false, "用户名不能为空");
            }

            if( string.IsNullOrWhiteSpace( newPassword ) ) {
                return (false, "新密码不能为空");
            }

            var user = await GetUserByUsername( username );
            if( user == null ) {
                return (false, "用户不存在");
            }

            user.Password = EncryptionHelper.EncryptText( newPassword );

            await UpdateUser( user );

            return (true, "密码修改成功");
        }

        #endregion

        #endregion
    }
}
