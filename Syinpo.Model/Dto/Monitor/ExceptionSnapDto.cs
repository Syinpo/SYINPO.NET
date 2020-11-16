using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Dto.Monitor {
   public class ExceptionSnapDto {
        public int Id {
            get; set;
        }

        public string TraceId {
            get; set;
        }

        public string ErrorSource {
            get; set;
        }
        public string ErrorDetail {
            get; set;
        }

        public DateTime CreateTime {
            get; set;
        }
    }
}
