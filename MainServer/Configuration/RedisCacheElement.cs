using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace MainServer.Configuration
{
    public class RedisCacheElement : ConfigurationElement
    {
        [ConfigurationProperty("host", IsRequired = true)]
        public string Host
        {
            get { return (string)this["host"]; }
        }

        [ConfigurationProperty("poolSize", IsRequired = true)]
        public int PoolSize
        {
            get { return (int)this["poolSize"]; }
        }

        [ConfigurationProperty("timeout", IsRequired = true)]
        public int Timeout
        {
            get { return (int)this["timeout"]; }
        }
    }
}
