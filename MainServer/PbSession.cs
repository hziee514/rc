using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace MainServer
{
    public class PbSession : AppSession<PbSession, BinaryRequestInfo>
    {
        protected override void OnSessionStarted()
        {
            base.OnSessionStarted();
        }

        protected override void HandleException(Exception e)
        {
            if (Logger.IsFatalEnabled)
            {
                Logger.Fatal("MainServer.PbSession.HandleException", e);
            }
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            base.OnSessionClosed(reason);
        }

        public void Send()
        {
        }
    }
}
