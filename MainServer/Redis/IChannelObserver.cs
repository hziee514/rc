using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MainServer.Redis
{
    public interface IChannelObserver
    {
        void OnAllMessage(string message);
        void OnGroupMessage(string groupId, string message);
        void OnSocketMessage(string socketId, string message);
    }
}
