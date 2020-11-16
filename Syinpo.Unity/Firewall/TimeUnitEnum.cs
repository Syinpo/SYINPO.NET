using System.ComponentModel.DataAnnotations;

namespace Syinpo.Unity.Firewall {

    public enum TimeUnitEnum : int {
        /// <summary>
        /// 秒
        /// </summary>
        [Display( Name = "秒" )]
        Second = 10,

        /// <summary>
        /// 分
        /// </summary>
        [Display( Name = "分" )]
        Minute = 20,

        /// <summary>
        /// 小时
        /// </summary>
        [Display( Name = "小时" )]
        Hours = 30,

        /// <summary>
        /// 天
        /// </summary>
        [Display( Name = "天" )]
        Day = 40,

        /// <summary>
        /// 当天
        /// </summary>
        [Display( Name = "当天" )]
        Today = 50,
    }
}
