using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MainServer
{
    public static class PbHelper
    {
        public const int HeaderSize = 4;

        public static byte[] Int2Bytes(int size)
        {
            var data = new byte[4];
            data[0] = (byte)(size >> 24 & 0xFF);
            data[1] = (byte)(size >> 16 & 0xFF);
            data[2] = (byte)(size >> 8 & 0xFF);
            data[3] = (byte)(size & 0xFF);
            return data;
        }

        public static int Bytes2Int(byte[] data, int offset = 0)
        {
            return (data[offset + 0] << 24) + (data[offset + 1] << 16) + (data[offset + 2] << 8) + data[offset + 3];
        }
    }
}
