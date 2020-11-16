using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;
using Syinpo.BusinessLogic.SignalR.Online;
using Syinpo.Core;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Model.Core.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Syinpo.Model;
using Microsoft.Extensions.Options;

namespace Syinpo.BusinessLogic.SignalR.Hubs {
    public abstract class HubBase : Hub {
        protected IOnlineManager OnlineManager {
            get;
        }
        protected ICurrent Current {
            get;
        }
        public ILogger<HubBase> Logger {
            get;
        }

        public SysOptions SysOptions {
            get;
        }


        protected HubBase( ICurrent current, IOnlineManager onlineManager, ILogger<HubBase> logger, IOptions<SysOptions> sysOptions ) {
            Current = current;
            OnlineManager = onlineManager;
            Logger = logger;
            SysOptions = sysOptions?.Value;
        }

        public override async Task OnConnectedAsync() {
            await base.OnConnectedAsync();

            try {
                //  connection.User?.Identity?.Name;
                var client = new OnlineClient(
                    Context.ConnectionId,
                    CommonHelper.GetClientIpAddress(),
                    Current.User != null ? Current.User.Id : (int?)null,
                    Current.Device != null ? Current.Device.Id : (int?)null,
                    SysOptions.SysName );

                OnlineManager.Add( client );

                //Logger.LogInformation( "一个客户端连接: " + client + ":::" + OnlineManager.GetAllClients().Count + ":::id" + Current.Device?.Id);
            }
            catch( Exception ex ) {
                Logger.LogError( "HubBase Error" + ex.ToString(), ex );
            }
        }

        public override async Task OnDisconnectedAsync( Exception exception ) {
            try {
                //Logger.LogInformation( "一个客户端断开: " + Context.ConnectionId + ":" + Current.Device?.Id + ":" + CommonHelper.GetClientIpAddress() );

                await base.OnDisconnectedAsync( exception );

                OnlineManager.Remove( Context.ConnectionId );
            }
            catch( Exception ex ) {
                Logger.LogError( "HubBase Error" + ex.ToString(), ex );
            }
        }


        protected RealtimeResponseObject Error( string errorMessage = "error", object errorData = null ) {
            var errorsRootObject = new RealtimeResponseObject {
                Message = errorMessage,
                Success = false,
                Data = errorData
            };

            return errorsRootObject;
        }

        protected RealtimeResponseObject Success( string message = "ok" ) {
            var errorsRootObject = new RealtimeResponseObject {
                Message = message,
                Success = true
            };

            return errorsRootObject;
        }


        protected string GetErrors( ValidationResult validationResult ) {
            var errors = new List<string>();

            if( !validationResult.IsValid )
                errors.AddRange( validationResult.Errors.Select( error => error.ErrorMessage ) );

            return string.Join( "|", errors );
        }

    }
}
