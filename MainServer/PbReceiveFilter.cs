using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.Common;

namespace MainServer
{
    public class PbReceiveFilter : FixedHeaderReceiveFilter<BinaryRequestInfo>
    {
        public PbReceiveFilter() : base(PbHelper.HeaderSize) { }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            return PbHelper.Bytes2Int(header, offset);
        }

        protected override BinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            //the key used for matching the processing command
            return new BinaryRequestInfo("PbMsg", bodyBuffer.CloneRange(offset, length));
        }
    }
}
