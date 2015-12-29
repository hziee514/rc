using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;

namespace MainServer.Command
{
    public class PbMsg : CommandBase<PbSession, BinaryRequestInfo>
    {
        public override void ExecuteCommand(PbSession session, BinaryRequestInfo requestInfo)
        {
            throw new NotImplementedException();
        }
    }
}
