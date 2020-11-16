using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Domain.RestApi {
    public class SuccessObject {
        public int Code { get; set; } = 0;

        public string Message {
            get; set;
        }

        public object Data {
            get;set;
        }
    }
}
