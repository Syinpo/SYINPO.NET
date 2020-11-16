using System;
using System.Collections.Generic;
using System.Text;
using Hangfire;
using Syinpo.BusinessLogic.Jobs;

namespace Syinpo.Unity.Hangfire {

    public static class HangfireRegister {
        public static void Register() {

            RecurringJob.AddOrUpdate<EventQueueJob>( a => a.Execute( new NullJobArgs { } ), "*/3 * * * * *" );

            RecurringJob.AddOrUpdate<EventQueueDeleteExpiryJob>( a => a.Execute( new NullJobArgs { } ), Cron.Daily( 6 ) );

        }
    }
}
