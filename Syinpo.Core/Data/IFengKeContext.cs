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
    public class SyinpoContext : BaseSyinpoContext<SyinpoContext> {
        public SyinpoContext( DbContextOptions<SyinpoContext> options ) : base( options ) {
        }
    }
}