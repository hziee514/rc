using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace MainServer.Configuration
{
    public class RedisChannelElement : ConfigurationElement
    {
        [ConfigurationProperty("globalNamespace", IsRequired = true)]
        public string GlobalNamespace
        {
            get { return (string)this["globalNamespace"]; }
        }

        [ConfigurationProperty("localNamespace", IsRequired = true)]
        public string localNamespace
        {
            get { return (string)this["localNamespace"]; }
        }

        [ConfigurationProperty("separator", IsRequired = true)]
        public char Separator
        {
            get { return (char)this["separator"]; }
        }

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
