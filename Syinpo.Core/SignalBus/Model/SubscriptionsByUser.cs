using System.Collections.Concurrent;

namespace Syinpo.Core.SignalBus.Model
{
  public class SubscriptionsByUser
  {
    /// <summary>
    /// Key是topic
    /// </summary>
    public ConcurrentDictionary<string, UserSubscription> SubscriptionsByTopic = new ConcurrentDictionary<string, UserSubscription>();

    public string ConnectionId;
  }
}
