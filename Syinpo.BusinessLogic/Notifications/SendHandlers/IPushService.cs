using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syinpo.Core.Domain.Poco;
using Syinpo.Model.Core.Notifications;
using Syinpo.Model.Dto.Devices;
using Syinpo.Model.Dto.Notifications;

namespace Syinpo.BusinessLogic.Notifications.SendHandlers
{
    public interface IPushService
    {
        /// <summary>
        /// 向移动端发送Token
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task SendToDeviceWithAccessToken(int deviceId);
    }
}