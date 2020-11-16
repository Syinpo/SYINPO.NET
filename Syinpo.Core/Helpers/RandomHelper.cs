using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Helpers
{
    public class RandomHelper
    {
        public static int GetRandomSeconds()
        {
            Random random = new Random();
            return random.Next(1, 3600);
        }

        public static int GetRandomMilliseconds(int min =5 ,int max=30)
        {
            Random random = new Random();
            return random.Next(min, max) * 1000;
        }

        /// <summary>
        /// 添加微信好友所需要间隔的时间
        /// </summary>
        /// <returns></returns>
        public static int AddWxSleepTime()
        {
            Random random = new Random();
            return 60000 + random.Next(1, 10);
        }

    }
}
