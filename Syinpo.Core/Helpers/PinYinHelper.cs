using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Helpers
{
    /// <summary>
    /// 中文转拼音帮助类
    /// </summary>
    public static class PinYinHelper
    {
        public static string ToPinYin(this string zhongwen)
        {
            if (string.IsNullOrWhiteSpace(zhongwen))
            {
                return null;
            }
            string result = string.Empty;
            foreach (char item in zhongwen)
            {
                try
                {
                    ChineseChar cc = new ChineseChar(item);
                    if (cc.Pinyins.Count > 0 && cc.Pinyins[0].Length > 0)
                    {
                        string temp = cc.Pinyins[0].ToString();
                        result += temp.Substring(0, temp.Length - 1);
                    }
                }
                catch (Exception)
                {
                    result += item.ToString();
                }
            }

            return result;
        }
    }
}
