using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.BusinessLogic.Jobs {
    [Serializable]
    public class IdJobArgs {
        public IdJobArgs( int id )
        {
            Id = id;
        }


        /// <summary>
        /// 实体主键
        /// </summary>
        public int Id {
            get; set;
        }
    }
}
