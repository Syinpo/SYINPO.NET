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

    /// <summary>
    /// 只读数据库上下文
    /// </summary>
    public class SyinpoReadOnlyContext : BaseSyinpoContext<SyinpoReadOnlyContext> {

        public SyinpoReadOnlyContext( DbContextOptions<SyinpoReadOnlyContext> options ) : base( options ) {
        }

        public override async Task<int> SaveChangesAsync() {
            await Task.CompletedTask;
            return 0;
        }
    }
}