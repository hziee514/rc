using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amib.Threading;
using MainServer.Configuration;

namespace MainServer
{
    public static class WorkExecuter
    {
        private static SmartThreadPool stp_ = new SmartThreadPool(
            SmartThreadPool.DefaultIdleTimeout,
            MainServerSection.Section.MaxTpSize,
            MainServerSection.Section.MinTpSize);

        static WorkExecuter()
        {
            stp_.Start();
        }

        public static SmartThreadPool STP
        {
            get { return stp_; }
        }

        public static void Shutdown()
        {
            stp_.WaitForIdle();
            stp_.Shutdown();
        }
    }
}
