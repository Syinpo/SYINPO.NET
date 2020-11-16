using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syinpo.Core.Monitor
{
    public interface IMonitorTimeDataManager
    {
        /// <summary>
        /// 添加分时数据
        /// </summary>
        /// <param name="timeGroup"></param>
        /// <param name="context"></param>
        /// <param name="data"></param>
        void AddData( string timeGroup, string context, object data );

        void AddRang(string timeGroup, List<Tuple<string, object>> data);

        Task Flush();
    }
}