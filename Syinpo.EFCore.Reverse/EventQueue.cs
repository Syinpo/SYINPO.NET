﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Syinpo.EFCore.Reverse
{
    public partial class EventQueue
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 隔离号
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 事件名称
        /// </summary>
        public string RouteName { get; set; }
        /// <summary>
        /// 头部信息
        /// </summary>
        public string Heads { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 重试次数
        /// </summary>
        public int Retry { get; set; }
        /// <summary>
        /// 成功时间
        /// </summary>
        public DateTime? OutTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}