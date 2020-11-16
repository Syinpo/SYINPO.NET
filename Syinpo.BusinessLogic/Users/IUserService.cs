using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syinpo.Core;
using Syinpo.Core.Domain.Poco;
using Syinpo.Model.Dto.Users;

namespace Syinpo.BusinessLogic.Users
{
    public interface IUserService
    {
        Task<PageList<User>> SearchUsers( DateTime? createTimeStart = null, DateTime? createTimeEnd = null, int partnerId = 0,
            int[] userRoleIds = null, int[] deptIds = null, string keywords = null, int pageIndex = 0, int pageSize = 10000 );

        Task DeleteUser( User user );
        Task<User> GetUserById( int userId );
        Task<List<User>> GetUsersByIds( int[] userIds );
        Task<User> GetUserByGuid( Guid userGuid );
        Task<User> GetUserByUsername( string username );
        Task InsertUser( User user );
        Task UpdateUser( User user );
        Task<UserDto> GetUserById_Pref(int id);
        Task<UserDto> GetUserByUsername_Pref(string userName);
        Task<(bool success, string errorMessage)> ValidateUser(string username, string clearTextPassword);
    }
}