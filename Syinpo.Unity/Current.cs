using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.Users;
using Syinpo.Core;
using Syinpo.Core.Caches;
using Syinpo.Core.Domain.Poco;
using Syinpo.Core.Extensions;
using Syinpo.Core.Helpers;
using Syinpo.Core.Mapper;
using Syinpo.Model;
using Syinpo.Model.Dto.Devices;
using Syinpo.Model.Dto.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Syinpo.Unity {
    public class Current : ICurrent {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IDeviceService _deviceService;
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions options;
        private readonly ICache _cache;
        private readonly ILogger<Current> _logger;

        public Current( IHttpContextAccessor httpContextAccessor, IUserService userService, IDeviceService deviceService, IMemoryCache memoryCache, ICache cache, ILogger<Current> logger ) {
            this._httpContextAccessor = httpContextAccessor;
            this._userService = userService;
            this._deviceService = deviceService;
            _memoryCache = memoryCache;
            options = new MemoryCacheEntryOptions();
            options.AbsoluteExpiration = DateTime.Now.AddDays( 3 );
            _cache = cache;
            _logger = logger;
        }

        private UserDto _cacheWebUser;
        public UserDto User {
            get {
                if( _cacheWebUser != null )
                    return _cacheWebUser;

                UserDto user = new UserDto { };
                if( _httpContextAccessor.HttpContext == null || !_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated )
                    return null;

                var claims = _httpContextAccessor.HttpContext.User.Claims.ToList();

                var usernameClaim = claims.FirstOrDefault( claim => claim.Type == "username" );
                if( usernameClaim != null ) {
                    //var userModel = _memoryCache.GetCacheFrom(Global.CacheKey.User + usernameClaim.Value, () =>
                    //{
                    //    var userEntity = _userService.GetUserByUsername(usernameClaim.Value).ConfigureAwait(false).GetAwaiter()
                    //    .GetResult();
                    //    return userEntity.MapTo<UserDto>();
                    //}, options);
                    //return userModel;
                    return _userService.GetUserByUsername_Pref( usernameClaim.Value ).ConfigureAwait( false ).GetAwaiter().GetResult();
                }
                if( user == null || !user.Approved || user.Deleted )
                    return user;

                _cacheWebUser = user;
                return _cacheWebUser;
            }
            set {
                _cacheWebUser = value;
            }
        }


        private DeviceDto _cacheDevice;
        public DeviceDto Device {
            get {
                if( _cacheDevice != null )
                    return _cacheDevice;

                int deviceId = 0;
                string deviceUuid = string.Empty;

                if( _httpContextAccessor.HttpContext == null )
                    return null;

                if( _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated ) {
                    var claims = _httpContextAccessor.HttpContext.User.Claims.ToList();

                    var deviceIdClaim = claims.FirstOrDefault( claim => claim.Type == "deviceId" );
                    if( deviceIdClaim != null )
                        int.TryParse( deviceIdClaim.Value, out deviceId );
                }

                if( deviceId == 0 ) {
                    deviceUuid = _httpContextAccessor.HttpContext.Request.Query[ "deviceUuid" ];
                }


                if( deviceId == 0 && string.IsNullOrEmpty( deviceUuid ) ) {
                    return null;
                }


                DeviceDto deviceDto = null;
                if( deviceId > 0 ) {
                    deviceDto = _deviceService.GetDeviceById_Pref( deviceId )
                        .ConfigureAwait( false )
                        .GetAwaiter()
                        .GetResult();
                }
                else {
                    deviceDto = _deviceService.GetDeviceByUuid_Pref( deviceUuid )
                        .ConfigureAwait( false )
                        .GetAwaiter()
                        .GetResult();


                    if( deviceDto == null ) {
                        var device = _deviceService.InsertGuestDevice( deviceUuid ).ConfigureAwait( false ).GetAwaiter()
                            .GetResult();
                        deviceDto = device.MapTo<DeviceDto>();
                    }
                }


                if( deviceDto == null || deviceDto.Deleted || deviceDto.Id == 0 )
                    return null;

                _cacheDevice = deviceDto;
                return _cacheDevice;
            }
            set {
                _cacheDevice = value;
            }
        }


        private DeviceDto _cacheHttpDevice;
        public DeviceDto HttpDevice {
            get {
                if( _cacheHttpDevice != null )
                    return _cacheHttpDevice;

                int deviceId = 0;
                string deviceUuid = string.Empty;

                if( _httpContextAccessor.HttpContext == null )
                    return null;

                if( _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated ) {
                    var claims = _httpContextAccessor.HttpContext.User.Claims.ToList();

                    var deviceIdClaim = claims.FirstOrDefault( claim => claim.Type == "deviceId" );
                    if( deviceIdClaim != null )
                        int.TryParse( deviceIdClaim.Value, out deviceId );

                    var deviceUuidClaim = claims.FirstOrDefault( claim => claim.Type == "deviceUuid" );
                    if( deviceUuidClaim != null )
                        deviceUuid = deviceUuidClaim.Value;
                }

                if( deviceId == 0 && string.IsNullOrEmpty( deviceUuid ) ) {
                    deviceUuid = _httpContextAccessor.HttpContext.Request.Query[ "deviceUuid" ];
                }


                if( deviceId == 0 && string.IsNullOrEmpty( deviceUuid ) ) {
                    return null;
                }


                DeviceDto deviceDto = new DeviceDto {
                    Id = deviceId,
                    DeviceUuid = deviceUuid
                };

                _cacheHttpDevice = deviceDto;
                return _cacheHttpDevice;
            }
            set {
                _cacheHttpDevice = value;
            }
        }


        private User _cacheDbUser;
        public User DbUser {
            get {
                if( _cacheDbUser != null )
                    return _cacheDbUser;

                User user = new User { };
                if( _httpContextAccessor.HttpContext == null || !_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated )
                    return null;

                var claims = _httpContextAccessor.HttpContext.User.Claims.ToList();

                var usernameClaim = claims.FirstOrDefault( claim => claim.Type == "username" );
                if( usernameClaim != null )
                    user = _userService.GetUserByUsername( usernameClaim.Value )
                        .ConfigureAwait( false )
                        .GetAwaiter()
                        .GetResult();
                //user = _userService.GetUserByUsername_Pref(usernameClaim.Value).ConfigureAwait(false).GetAwaiter()
                //    .GetResult()?.MapTo<User>();


                if( user == null || !user.Approved || user.Deleted )
                    return user;

                _cacheDbUser = user;
                return _cacheDbUser;
            }
            set {
                _cacheDbUser = value;
            }
        }



    }
}
