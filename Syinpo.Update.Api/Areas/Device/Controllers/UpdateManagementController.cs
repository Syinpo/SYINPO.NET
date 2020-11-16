using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Devices;
using Syinpo.Core.Data;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Model;
using Syinpo.Unity.AspNetCore.Attributes;
using Syinpo.Unity.AspNetCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;

namespace Syinpo.Update.Api.Areas.Device.Controllers {
    [SwaggerTag( "设备更新" )]
    [DeviceAuthorize]
    public class UpdateManagementController : BaseApiController {
        #region 字段
        private readonly IDeviceService _deviceService;
        private readonly ICurrent _current;
        private readonly IDbContext _dbContext;
        private readonly IConfiguration _configuration;
        #endregion

        #region 构造函数

        public UpdateManagementController( IDeviceService deviceService, ICurrent current,
            IDbContext dbContext,IConfiguration configuration ) {
            _deviceService = deviceService;
            _current = current;
            _dbContext = dbContext;
            _configuration = configuration;
        }
        #endregion


        ///// <summary>
        ///// 检查更新
        ///// </summary>
        ///// <param name="request">入参</param>
        ///// <returns>返回Response</returns>
        ///// <remarks>
        ///// 检查更新
        ///// </remarks>
        //[HttpPost]
        //[Route( "/api/device/update/check" )]
        //[SwaggerResponse( 0, "成功", typeof( Response<UpdateCheckResult> ) )]
        //[SwaggerResponse( -1, "失败", typeof( ErrorObject ) )]
        //public async Task<IActionResult> Check( CheckUpdateParametersModel request ) {
        //    if ( _current.Device == null || _current.Device.Deleted)
        //        return Error( propertyKey: "Device", errorMessage: "设备不存在" );

        //    if( string.IsNullOrEmpty( request.AppName ) ) {
        //        return Error( propertyKey: "AppName", errorMessage: "AppName不能为空" );
        //    }
        //    if( string.IsNullOrEmpty( request.Version ) ) {
        //        return Error( propertyKey: "Version", errorMessage: "Version不能为空" );
        //    }


        //    var item = _upgradeService.GetAppApkCategoryList().FirstOrDefault( f => f.AppName == request.AppName );
        //    if( item == null ) {
        //        return Error( propertyKey: "app", errorMessage: "app不能为空" );
        //    }

        //    var majorVerNumber = 0;
        //    var minorVerNumber = 0;
        //    var buildVerNumber = 0;
        //    var revisionVerNumber = 0;

        //    if( !string.IsNullOrEmpty( request.Version ) ) {
        //        Regex pattern = new Regex( @"^(\d+\.)?(\d+\.)?(\*|\d+)$" );
        //        Match m = pattern.Match( request.Version );
        //        if( !m.Success ) {
        //            return Error( propertyKey: "Version", errorMessage: "VerName错误, 按照格式 1.10.1" );
        //        }

        //        majorVerNumber = Convert.ToInt32( m.Groups[ 1 ].Value.Replace( ".", "" ) );
        //        minorVerNumber = Convert.ToInt32( m.Groups[ 2 ].Value.Replace( ".", "" ) );
        //        buildVerNumber = Convert.ToInt32( m.Groups[ 3 ].Value.Replace( ".", "" ) );
        //        revisionVerNumber = 0;
        //    }

        //    var update = await _upgradeService.GetLast( _current.Device, item );
        //    if( update == null ) {
        //        return Success( new UpdateCheckResult { NeedUpdate = false }, "服务器上没有找到可用的版本" );

        //    }

        //    Version v1 = new Version( majorVerNumber, minorVerNumber, buildVerNumber );
        //    Version v2 = new Version( update.MajorVerNumber, update.MinorVerNumber, update.BuildVerNumber );

        //    switch( v1.CompareTo( v2 ) ) {
        //        case 0:
        //            return Success( new UpdateCheckResult { NeedUpdate = false }, "已经是最新版本，客户端与服务器版本一致" );
        //        case 1:
        //            return Success( new UpdateCheckResult { NeedUpdate = false }, "已经是最新版本,且客户端版本比服务器的要新" );
        //    }


        //    return Success( new UpdateCheckResult {
        //        NeedUpdate = true,
        //        DownloadId = update.MediaId,
        //        LastTime = update.CreateTime,
        //        LastVersion = update.VerName,
        //        AppName = update.AppName,
        //        AutoInstall = true,
        //    } , update.UploadMemo);
        //}
    }
}