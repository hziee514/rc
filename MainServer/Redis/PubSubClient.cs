using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;
using MainServer.Configuration;
using SuperSocket.SocketBase.Logging;

namespace MainServer.Redis
{
    public class PubSubClient
    {
        private static PooledRedisClientManager pool_;

        private static readonly string gns_ = MainServerSection.Section.RedisChannel.GlobalNamespace;
        private static readonly string lns_ = MainServerSection.Section.RedisChannel.localNamespace;

        public static readonly char Separator = MainServerSection.Section.RedisChannel.Separator;

        private static readonly string gch_pattern_ = gns_ + Separator + "*";
        private static readonly string lch_pattern_ = lns_ + Separator + "*";

        private static readonly string lch_format_ = "" + Separator + "socket" + Separator;

        private static readonly string gch_all_ = gns_ + Separator + "all" + Separator;
        private static readonly string gch_group_prefix_ = gns_ + Separator + "group" + Separator;
        private static readonly string lch_socket_prefix_ = lns_ + Separator + "socket" + Separator;
        private static readonly string lch_exit_ = lns_ + Separator + "exit" + Separator;

        private static readonly string exit_msg_ = MainServerSection.Section.InstanceName;

        static PubSubClient()
        {
            InitRedisClientPool();
        }

        public static void InitRedisClientPool()
        {
            //auto started
            pool_ = new PooledRedisClientManager(
                MainServerSection.Section.RedisChannel.PoolSize,
                MainServerSection.Section.RedisChannel.Timeout,
                MainServerSection.Section.RedisChannel.Host
                );
        }

        public ILog Logger { get; set; }

        private IChannelObserver observer_ = null;

        public PubSubClient(IChannelObserver observer)
        {
            observer_ =observer;
            Start();
        }

        private void HandlerChannelMessage(string channel, string msg)
        {
            if (channel == gch_all_)
            {
                observer_.OnAllMessage(msg);
            }
            else if (channel.StartsWith(lch_socket_prefix_))
            {
                var id = channel.Substring(lch_socket_prefix_.Length);
                observer_.OnSocketMessage(id, msg);
            }
            else if (channel.StartsWith(gch_group_prefix_))
            {
                var id = channel.Substring(gch_group_prefix_.Length);
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
                        if (channel == lch_exit_ && msg == exit_msg_)
                        {
                            sub.UnSubscribeFromChannelsMatching(gch_pattern_, lch_pattern_);
                        }
                        else
                        {
                            //process with another thread, nonblock
                            WorkExecuter.STP.QueueWorkItem(() => { HandlerChannelMessage(channel, msg); });
                        }
                    };
                    sub.SubscribeToChannelsMatching(gch_pattern_, lch_pattern_);
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
            WorkExecuter.STP.QueueWorkItem(SubscribeChannel);
        }

        public void Stop()
        {
            PublishToExit();
        }

        public void PublishToAll(string message)
        {
            Publish(gch_all_, message);
        }

        public void PublishToGroup(string groupId, string message)
        {
            Publish(gch_group_prefix_ + groupId, message);
        }

        public void PublishToSocket(string ns, string socketId, string message)
        {
            Publish(ns + lch_format_ + socketId, message);
        }

        public void PublishToExit()
        {
            Publish(lch_exit_, exit_msg_);
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
