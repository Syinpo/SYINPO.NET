using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Container {
    public  class DeviceMessage<T> : MediatR.INotification {
        public int DeviceId {
            get;set;
        }

        public T Data {

            get;
            set;
        }
    }
}
