using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Syinpo.Core.Data {
    public partial class BaseSyinpoContext<T>  {
        partial void OnModelCreatingPartial( ModelBuilder modelBuilder ) {

        }
    }
}
