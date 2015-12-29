using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;
using MainServer.Configuration;
using SuperSocket.SocketBase.Logging;
using System.Threading;
using Amib.Threading;

namespace MainServer.Redis
{
    public class PubSubClient
    {
        private static PooledRedisClientManager pool_;

        private static string ns_ = MainServerConfiguration.Section.RedisChannel.Namespace;
        public static readonly char Separator = MainServerConfiguration.Section.RedisChannel.Separator;

        private static string ch_prefix_ = ns_ + Separator;
        private static string ch_pattern_ = ch_prefix_ + "*";

        private static readonly string ch_all_ = ch_prefix_ + "all" + Separator;

        private static readonly string ch_group_prefix_ = ch_prefix_ + "group" + Separator;
        private static readonly string ch_socket_prefix_ = ch_prefix_ + "socket" + Separator;

        private static readonly string ch_exit_ = ch_prefix_ + "exit" + Separator;

        private static readonly string exit_msg_ = MainServerConfiguration.Section.InstanceName;

        static PubSubClient()
        {
            InitRedisClientPool();
        }

        public static void InitRedisClientPool()
        {
            //auto started
            pool_ = new PooledRedisClientManager(
                MainServerConfiguration.Section.RedisChannel.PoolSize,
                MainServerConfiguration.Section.RedisChannel.Timeout,
                MainServerConfiguration.Section.RedisChannel.Host
                );
        }

        public ILog Logger { get; set; }

        private SmartThreadPool stp_ = new SmartThreadPool(SmartThreadPool.DefaultIdleTimeout, 5, 1);
        private IChannelObserver observer_ = null;

        public PubSubClient(IChannelObserver observer)
        {
            observer_ =observer;

            stp_.Start();
            Start();
        }

        private void HandlerChannelMessage(string channel, string msg)
        {
            if (channel == ch_all_)
            {
                observer_.OnAllMessage(msg);
            }
            else if (channel.StartsWith(ch_socket_prefix_))
            {
                var id = channel.Substring(ch_socket_prefix_.Length);
                observer_.OnSocketMessage(id, msg);
            }
            else if (channel.StartsWith(ch_group_prefix_))
            {
                var id = channel.Substring(ch_group_prefix_.Length);
                observer_.OnGroupMessage(id, msg);
            }
            else
            {
                if (Logger.IsWarnEnabled)
                {
                    Logger.WarnFormat("unknown channel {0} msg {1}", channel, msg);
                }
            }
        }

        private void SubscribeChannel()
        {
            try
            {
                using (var redis = pool_.GetClient())
                using (var sub = redis.CreateSubscription())
                {
                    sub.OnMessage = (channel, msg) =>
                    {
                        if (Logger.IsDebugEnabled)
                        {
                            Logger.DebugFormat("sub channel {0} msg {1}", channel, msg);
                        }
                        if (channel == ch_exit_ && msg == exit_msg_)
                        {
                            sub.UnSubscribeFromChannelsMatching(ch_pattern_);
                        }

                        //send to threadpool, nonblock
                        stp_.QueueWorkItem(() => { HandlerChannelMessage(channel, msg); });
                    };
                    sub.SubscribeToChannelsMatching(ch_pattern_);
                }
            }
            catch (Exception e)
            {
                if (Logger.IsErrorEnabled)
                {
                    Logger.Error("PubSubClient.SubscribeChannel", e);
                }
                Start();
            }
        }

        public void Start()
        {
            stp_.QueueWorkItem(SubscribeChannel);
        }

        public void Stop()
        {
            PublishToExit();
            stp_.WaitForIdle();
        }

        public void PublishToAll(string message)
        {
            Publish(ch_all_, message);
        }

        public void PublishToGroup(string groupId, string message)
        {
            Publish(ch_group_prefix_ + groupId, message);
        }

        public void PublishToSocket(string socketId, string message)
        {
            Publish(ch_socket_prefix_ + socketId, message);
        }

        public void PublishToExit()
        {
            Publish(ch_exit_, exit_msg_);
        }

        private void Publish(string channel, string message)
        {
            using (var redis = pool_.GetClient())
            {
                redis.PublishMessage(channel, message);
            }
        }
    }
}
