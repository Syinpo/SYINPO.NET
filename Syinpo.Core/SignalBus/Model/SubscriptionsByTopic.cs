using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.SignalBus.Model {
    public class SubscriptionsByTopic {
        /// <summary>
        /// Key是连接ID
        /// </summary>

        public ConcurrentDictionary<string, SubscriptionsByUser> SubscriptionsByUser = new ConcurrentDictionary<string, SubscriptionsByUser>();

        public string Topic;
    }
}
