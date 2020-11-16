using System;
using System.ComponentModel;
using System.Reflection;

namespace Syinpo.Core.Domain.RestApi
{
    public class Enums
    {
        public static class PageConfig
        {
            public const int PageSize = 10;
        }

        public enum MessageType
        {
            /// <summary>
            /// 文本
            /// </summary>
            Text = 1,
            /// <summary>
            /// 图片
            /// </summary>
            Image = 2,
            /// <summary>
            /// 视频
            /// </summary>
            Video = 3,
            /// <summary>
            /// 语音
            /// </summary>
            Voice = 4
        }

        public enum WeiXinType
        {
            /// <summary>
            /// 微信
            /// </summary>
            [Description("base")]
            Base = 0,
            /// <summary>
            /// 群
            /// </summary>
            [Description("group")]
            Group = 5,
            /// <summary>
            /// 公众号
            /// </summary>
            [Description("公众号")]
            Public = 10
        } 
    }

}
