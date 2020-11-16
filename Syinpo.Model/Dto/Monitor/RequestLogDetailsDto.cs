using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Dto.Monitor {
    public class RequestLogDetailsDto {
        public RequestLogDetailsDto()
        {
            SqlSnaps = new List<SqlSnapDto>();
            ExceptionSnaps = new List<ExceptionSnapDto>();
        }

        /// <summary>
        /// 请求信息
        /// </summary>
        public RequestLogDto RequestLog {
            get;set;
        }

        /// <summary>
        /// 响应信息
        /// </summary>
        public ResponseSnapDto ResponseSnap {
            get;set;
        }

        /// <summary>
        /// 数据访问
        /// </summary>
        public List<SqlSnapDto> SqlSnaps {
            get; set;
        }


        /// <summary>
        /// 未处理异常
        /// </summary>
        public List<ExceptionSnapDto> ExceptionSnaps {
            get; set;
        }
    }
}
