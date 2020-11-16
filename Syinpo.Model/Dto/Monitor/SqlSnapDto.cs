using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Dto.Monitor {
    public class SqlSnapDto {
        public int Id {
            get; set;
        }

        public string TraceId {
            get; set;
        }

        public string SqlRaw {
            get; set;
        }
        public string SqlBody {
            get; set;
        }
        public long Elapsed {
            get; set;
        }
        public DateTime CreateTime {
            get; set;
        }
    }
}
