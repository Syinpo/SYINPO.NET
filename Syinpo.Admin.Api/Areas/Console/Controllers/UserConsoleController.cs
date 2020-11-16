using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.Notifications.SendHandlers;
using Syinpo.BusinessLogic.Users;
using Syinpo.Core;
using Syinpo.Core.Data;
using Syinpo.Core.Domain.Poco;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Core.Helpers;
using Syinpo.Core.Mapper;
using Syinpo.Model;
using Syinpo.Model.Dto.Users;
using Syinpo.Model.Request.Users;
using Syinpo.Unity.AspNetCore.Attributes;
using Syinpo.Unity.AspNetCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Safety;
using Syinpo.Core.Caches;
using Syinpo.BusinessLogic.Caches;

namespace Syinpo.Admin.Api.Areas.Console.Controllers
{

    [SwaggerTag("用户管理")]
    public class UserConsoleController : BaseConsoleApiController {

        #region 字段

        private readonly ICurrent _current;
        private readonly IDeviceService _deviceService;
        private readonly IUserService _userService;
        private readonly IPushService _pushService;
        private readonly DbContextFactory _DbContextFactory;
        private readonly ICache _cache;
        #endregion 字段

        #region 构造函数

        public UserConsoleController(IDeviceService deviceService, ICurrent current, IUserService userService, IPushService pushService, DbContextFactory DbContextFactory, ICache cache)
        {
            _current = current;
            _deviceService = deviceService;
            _userService = userService;
            _pushService = pushService;
            _DbContextFactory = DbContextFactory;
            _cache = cache;
        }

        #endregion 构造函数

        #region 公共方法

        private UserForConsoleDto PreUserModel(Core.Domain.Poco.User user)
        {
            if (user == null)
                throw new Exception("user is null");

            var model = user.MapTo<UserForConsoleDto>();
            return model;
        }

        #endregion 公共方法

        #region 用户

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize()]
        [Route("/api/console/user/list")]
        [SwaggerResponse(0, "成功", typeof(Response<List<UserForConsoleDto>>))]
        [SwaggerResponse(-1, "失败", typeof(ErrorObject))]
        public async Task<IActionResult> GetUserList(UserListParametersModel request)
        {

            return await _DbContextFactory.UseTransaction(async () =>
            {
                if (request == null)
                    return Error(propertyKey: "request", errorMessage: "request为空");

                int page = request.Page;
                int pageSize = request.PageSize;

                if (page < 0)
                    page = 1;
                if (pageSize < 0)
                    pageSize = 20;

                var users = await _userService.SearchUsers(null, null, 0, request.UserRoleIds, null, request.Keyword, page - 1, pageSize);

                var result = users.Select(PreUserModel).ToList();
                return Success(new PageResult<UserForConsoleDto>
                {
                    Results = result,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = users.TotalCount,
                    TotalPages = users.TotalPages,
                });
            });
        }

        /// <summary>
        /// 获取一个用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorize]
        [Route("/api/console/user/{id}")]
        [SwaggerResponse(0, "成功", typeof(Response<UserForConsoleDto>))]
        [SwaggerResponse(-1, "失败", typeof(ErrorObject))]
        public async Task<IActionResult> Get(int id)
        {
            if (id == 0)
            {
                ModelState.AddModelError(string.Empty, "Id不能为空");
            }

            if (!ModelState.IsValid)
            {
                return Error();
            }

            var user = await _userService.GetUserById(id);
            var model = PreUserModel(user);
            return Success(model);
        }

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorize]
        [Route("/api/console/user/name/{name}")]
        [SwaggerResponse(0, "成功", typeof(Response<UserDto>))]
        [SwaggerResponse(-1, "失败", typeof(ErrorObject))]
        public async Task<IActionResult> GetUserByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError(string.Empty, $"{nameof(name)}不能为空");
            }

            if (!ModelState.IsValid)
            {
                return Error();
            }

            var user = await _userService.GetUserByUsername(name);
            return Success(user.MapTo<UserDto>());
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        [Route("/api/console/user/add")]
        [SwaggerResponse(0, "成功", typeof(Response<UserForConsoleDto>))]
        [SwaggerResponse(-1, "失败", typeof(ErrorObject))]
        public async Task<IActionResult> Add(UserForEditDto model)
        {
            return await _DbContextFactory.UseTransaction(async () =>
            {

                if (string.IsNullOrEmpty(model.Username))
                    return Error(propertyKey: "username", errorMessage: "用户名不能为空");

                if ((await _userService.GetUserByUsername(model.Username)) != null)
                {
                    return Error(propertyKey: "user", errorMessage: "用户名已经被其它合作伙伴注册");
                }

                var user = new User
                {
                    PartnerId = 0,
                    ParentUserId = 0,
                    UserGuid = Guid.NewGuid(),
                    Username = model.Username,
                    Password = model.Password,
                    DisplayName = model.DisplayName,
                    RealName = model.RealName,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    Approved = model.Approved,
                    Deleted = false,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    LastLoginDate = null,
                };
                await _userService.InsertUser(user);

                //密码
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    //var result = await _userService.ChangePassword(model.Username, model.Password);
                    //if (!result.success)
                    //{
                    //    return Error(propertyKey: "password", errorMessage: "用户创建成功但密码设置失败" + result.errorMessage);
                    //}
                }

                await _userService.UpdateUser(user);

                return Success(PreUserModel(user));

            });


        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        [Route("/api/console/user/update")]
        [SwaggerResponse(0, "成功", typeof(Response<UserForConsoleDto>))]
        [SwaggerResponse(-1, "失败", typeof(ErrorObject))]
        public async Task<IActionResult> Update(UserForEditDto model)
        {
            return await _DbContextFactory.UseTransaction(async () =>
            {
                var user = await _userService.GetUserById(model.Id);
                if (user == null || user.Deleted )
                    return Error(propertyKey: "user", errorMessage: "用户不能存在");


                user.DisplayName = model.DisplayName;
                user.Approved = model.Approved;
                user.Email = model.Email;
                user.UpdateTime = DateTime.Now;
                user.Mobile = model.Mobile;
                user.RealName = model.RealName;
                await _userService.UpdateUser(user);

                return Success(PreUserModel(user));

            });


        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        [Route("/api/console/user/changepassword")]
        [SwaggerResponse(0, "成功", typeof(Response<bool>))]
        [SwaggerResponse(-1, "失败", typeof(ErrorObject))]
        public async Task<IActionResult> ChangePassword(UserChangPasswordParametersModel model)
        {
            return null;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        [Route("/api/console/user/delete/{id}")]
        [SwaggerResponse(0, "成功", typeof(Response<bool>))]
        [SwaggerResponse(-1, "失败", typeof(ErrorObject))]
        public async Task<IActionResult> Delete(int id)
        {
            return await _DbContextFactory.UseTransaction(async () =>
            {
                var user = await _userService.GetUserById(id);
                if (user == null )
                    return Error(propertyKey: "user", errorMessage: "用户不能存在");

                await _userService.DeleteUser(user);

                return Success(true);

            });


        }

        #endregion 用户
    }
}