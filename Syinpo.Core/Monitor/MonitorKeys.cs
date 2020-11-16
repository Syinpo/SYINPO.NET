using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Monitor {
    public class MonitorKeys {

        public static readonly string request_type = "request.type";

        // 是否有异常错误
        public static readonly string request_error = "request.error";
        public static readonly string request_errordetail = "request.errordetail";

        //
        public static readonly string response_success = "response.success";
        public static readonly string response_contentlength = "response.contentlength";
        public static readonly string response_body = "response.body";
        public static readonly string response_statuscode = "response.statuscode";

        // 耗时
        public static readonly string response_elapsed = "response.elapsed";

        // trace
        public static readonly string trace_id = "trace.id";
        public static readonly string service_name = "service.name";
        public static readonly string trace_parentid = "trace.parentid";
        public static readonly string trace_spanid = "trace.spanid";
        public static readonly string trace_duration = "trace.duration";
        public static readonly string trace_spanevent = "trace.spanevent";
        public static readonly string trace_data = "trace.data";
        public static readonly string cap_head = "cap.head";

        // sql
        public static readonly string sql_raw = "sql.raw";
        public static readonly string sql_body = "sql.body";
    }
}
