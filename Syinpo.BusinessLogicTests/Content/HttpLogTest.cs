using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CachingFramework.Redis;
using Syinpo.BusinessLogic.Caches;
using Syinpo.BusinessLogic.Safety;
using Syinpo.BusinessLogic.SignalR.Online;
using Syinpo.Core;
using Syinpo.Core.Caches;
using Syinpo.Core.Helpers;
using Syinpo.Core.Monitor.ClrModule;
using Syinpo.Core.Monitor.PackModule;
using Syinpo.Model.Core.Content;
using Syinpo.Tests.Core;
using Syinpo.Unity.Redis;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NUnit.Framework;
using Sprache;

namespace Syinpo.Tests.BusinessLogic.Content {
    [TestFixture()]
    public class HttpLogTest : BaseTest {

        [Test()]
        public async Task Amr_Test() {
            var logStore = Syinpo.Core.IoC.Resolve<IPackStore<HttpLog>>();
            for( int i = 0; i < 91; i++ ) {
                try {

                    var key = DateTime.Now.ToString( "yyyyMMddHHmm" );
                    logStore.AddQueue(  new HttpLog { RequestPath = i.ToString() } );
                    System.Threading.Thread.Sleep( 1000 );
                }
                catch( Exception ex ) {
                    ;
                }
            }

            var a = 0;
        }


        [Test()]
        public async Task Amr_Test2() {

            try {
                var userPrincipal = IoC.Resolve<ITokenService>().GetPrincipalFromExpiredToken( "eyJhbGciOiJSUzI1NiIsInR5cCI6ImF0K2p3dCJ9.eyJuYmYiOjE1ODg4NjA1NTgsImV4cCI6MTY3NTI2MDU1OCwiaXNzIjoiaHR0cHM6Ly9hdXRoLmlmZW5na2UuY29tIiwiYXVkIjoiYXBpIiwiY2xpZW50X2lkIjoiY2xpZW50Iiwic3ViIjoiMSIsImF1dGhfdGltZSI6MTU4ODg2MDU1OCwiaWRwIjoibG9jYWwiLCJ1c2VybmFtZSI6ImFkbWluIiwic3VwcGVyIjoidHJ1ZSIsInNjb3BlIjpbImFwaSIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJwd2QiXX0.YVjmWVEBSoJSSepcXm6lb83BGeZkUkB1EBCDnLLUJKiX-g-qewL1BRFLmLvClIm3mll1tSkPxuGd1GKoakcmiQrSsXsyYz6yqDhx_vZxOKfZjGPjWIVidH79PcfG46Xqe4XXIxNqmoDCY84QdNEj2F-n90DGirWTldygpUftaNcraFQIHSyrY1tXyoZkvdh3QAovj6dWwbxc8ECHQJ69YvF6bVrpWoG2Af0z-zbiPLPQdEeXM-SE-5R46UAL8yn4w-Id8R7B1T4X1iSu2yLRfUKD8qBB1gT-d72IccQE-ZDMgdKcDAZYUs0zdx16_9slhrqDT99GPhXADNGnJZ006w", out bool expired );
                ;
            }
            catch( Exception ex ) {
                ;
            }
        }

        public static bool TryParseJson( string obj, out JObject result ) {
            try {
                result = JObject.Parse( obj );
                return true;
            }
            catch( Exception ) {
                result = default( JObject );
                return false;
            }
        }


        [Test()]
        public async Task Amr_Test3() {

            try {
                JsonHelper.ToPack( "c:\\11\\1.log", new List<Tuple<string, object>>
                {
                    new Tuple<string, object>( "http", 1 ),
                    new Tuple<string, object>( "cpu", 0.2 )
                } );

                var aa = JsonHelper.ToData<List<Tuple<string, object>>>("c:\\11\\1.log");


                while( true ) {

                    var cpuStats = CpuHelpers.UsagePercent;

                    var Gen0CollectCount = GCHelpers.Gen0CollectCount;
                    var Gen1CollectCount = GCHelpers.Gen1CollectCount;
                    var Gen2CollectCount = GCHelpers.Gen2CollectCount;
                    var HeapMemory = GCHelpers.TotalMemory;

                    await Task.Delay( 1000 );
                }
            }
            catch( Exception ex ) {
                ;
            }
        }


        [Test()]
        public async Task Amr_Test44() {
            try
            {
                var cache = IoC.Resolve<ICache>();
                await cache.LockWith("aa", TimeSpan.FromSeconds(5), async () => { Console.WriteLine("aa"); });
            }
            catch( Exception ex ) {
                ;
            }
        }
    }
}
