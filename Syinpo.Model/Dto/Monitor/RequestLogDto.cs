using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Dto.Monitor {
    public class RequestLogDto {
        public RequestLogDto()
        {
            Tags = new List<RequestLogTagDto>();
        }

        /// <summary>
        /// 主键
        /// </summary>
        public int Id {
            get; set;
        }

        /// <summary>
        /// 跟踪标识
        /// </summary>
        public string TraceId {
            get; set;
        }

        /// <summary>
        /// 客户端远程IP
        /// </summary>
        public string RemoteIpAddress {
            get; set;
        }

        /// <summary>
        /// 客户端远程端口
        /// </summary>
        public int RemotePort {
            get; set;
        }

        /// <summary>
        /// 是否授权设备与用户
        /// </summary>
        public bool IdentityIsAuthenticated {
            get; set;
        }
        /// <summary>
        /// 授权身份名
        /// </summary>
        public string IdentityName {
            get; set;
        }

        /// <summary>
        /// 请求方法
        /// </summary>
        public string RequestMethod {
            get; set;
        }
        /// <summary>
        /// 请求协议
        /// </summary>
        public string RequestScheme {
            get; set;
        }

        /// <summary>
        /// 请求路由
        /// </summary>
        public string RequestPath {
            get; set;
        }
        /// <summary>
        /// 请求查询参数
        /// </summary>
        public string RequestQueryString {
            get; set;
        }
        /// <summary>
        /// 请求内容协议
        /// </summary>
        public string RequestContentType {
            get; set;
        }
        /// <summary>
        /// 数据大小
        /// </summary>
        public long? RequestContentLength {
            get; set;
        }

        /// <summary>
        /// 域名
        /// </summary>
        public string RequestHost {
            get; set;
        }
        /// <summary>
        /// 请求头
        /// </summary>
        public string RequestHead {
            get; set;
        }

        /// <summary>
        /// 请求体
        /// </summary>
        public string RequestBody {
            get; set;
        }

        /// <summary>
        /// 请求发起时间
        /// </summary>
        public DateTime RequestTime {
            get; set;
        }


        /// <summary>
        /// 请求类型， 10-http， 20-signalr
        /// </summary>
        public int RequestType {
            get; set;
        }

        /// <summary>
        /// 请求组
        /// </summary>
        public long RequestGroup {
            get; set;
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string Operation {
            get; set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime {
            get; set;
        }

        /// <summary>
        /// 标签(s)
        /// </summary>
        public List<RequestLogTagDto> Tags {
            get;set;
        }
    }

    public class RequestLogTagDto {
        /// <summary>
        /// 标签名
        /// </summary>
        public string Tag {
            get; set;
        }

        /// <summary>
        /// 标签类型(目前返回颜色值)
        /// info-一般
        /// warn-警告（建议黄色）
        /// error-错误（建议红色）
        ///
        /// </summary>
        public string TagColor {
            get;set;
        }
    }
}
