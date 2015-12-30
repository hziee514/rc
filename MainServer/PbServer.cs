using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System.Configuration;
using MainServer.Configuration;
using MainServer.Redis;

namespace MainServer
{
    public class PbServer : AppServer<PbSession, BinaryRequestInfo>, IChannelObserver
    {
        public PbServer()
            : base(new DefaultReceiveFilterFactory<PbReceiveFilter, BinaryRequestInfo>())
        { }

        PubSubClient pubsubClient;

        protected override void OnStarted()
        {
            pubsubClient = new PubSubClient(this)
            {
                Logger = this.Logger
            };

            base.OnStarted();
        }

        protected override void OnStopped()
        {
            base.OnStopped();

            pubsubClient.Stop();

            WorkExecuter.Shutdown();
        }

        public void OnAllMessage(string message)
        {
            //throw new NotImplementedException();
        }

        public void OnGroupMessage(string groupId, string message)
        {
            //throw new NotImplementedException();
        }

        public void OnSocketMessage(string socketId, string message)
        {
            //throw new NotImplementedException();
        }
    }
}
