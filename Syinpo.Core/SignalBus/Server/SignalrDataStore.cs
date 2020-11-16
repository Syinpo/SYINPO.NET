using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syinpo.Core.SignalBus.Model;

namespace Syinpo.Core.SignalBus.Server {
    public class SignalrDataStore {
        public static ConcurrentDictionary<string, SubscriptionsByTopic> SubscriptionsByTopic { get; set; } = new ConcurrentDictionary<string, SubscriptionsByTopic>();


        public static void AddUser( UserSubscription user ) {
            // 获取某个主题订阅
            SubscriptionsByTopic subscriptionsByTopic;
            SubscriptionsByTopic.TryGetValue( user.Topic, out subscriptionsByTopic );

            // 如果主题不存在，新添加
            if( subscriptionsByTopic == null ) {
                subscriptionsByTopic = new SubscriptionsByTopic() {
                    Topic = user.Topic
                };

                var wasAdded = SubscriptionsByTopic.TryAdd( user.Topic, subscriptionsByTopic );
            }


            // 获取某个主题用户的全部订阅
            SubscriptionsByUser subscriptionsByUser;
            subscriptionsByTopic.SubscriptionsByUser.TryGetValue( user.ConnectionId, out subscriptionsByUser );

            // 主题用户不存在，新添加
            if( subscriptionsByUser == null ) {
                subscriptionsByUser = new SubscriptionsByUser() {
                    ConnectionId = user.ConnectionId,
                };

                var wasAdded = subscriptionsByTopic.SubscriptionsByUser.TryAdd( user.ConnectionId, subscriptionsByUser );

            }

            // 用户订阅值同步
            subscriptionsByUser.SubscriptionsByTopic.AddOrUpdate( user.Topic, user, ( key, oldValue ) => oldValue = user );
        }

        public static void RemoveTopic( string connectionId, string topic ) {
            foreach( var topicSubscription in SubscriptionsByTopic ) {
                if( topicSubscription.Value == null ) {
                    continue;
                }

                SubscriptionsByUser userSubscription;
                topicSubscription.Value.SubscriptionsByUser.TryGetValue( connectionId, out userSubscription );
                if( userSubscription != null ) {
                    userSubscription.SubscriptionsByTopic.TryRemove( topic, out _ );
                }
            }
        }

        public static async Task RemoveConnection( string connectionId ) {
            foreach( var topicSubscription in SubscriptionsByTopic ) {
                if( topicSubscription.Value == null ) {
                    continue;
                }

                if( topicSubscription.Value.SubscriptionsByUser.ContainsKey( connectionId ) ) {
                    var removesuccess = topicSubscription.Value.SubscriptionsByUser.TryRemove( connectionId, out _ );
                    if( !removesuccess ) {
                        var newRand = new Random();
                        await Task.Delay( newRand.Next( 50 ) );
                        await RemoveConnection( connectionId );
                    }
                }
            }

            return;
        }
    }
}
