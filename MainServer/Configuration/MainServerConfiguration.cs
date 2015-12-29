using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace MainServer.Configuration
{
    public sealed class MainServerConfiguration : ConfigurationSection
    {
        private const string SECTION_NAME = "mainServer";

        private static MainServerConfiguration section_;

        public static MainServerConfiguration Section
        {
            get
            {
                if (section_ == null)
                {
                    section_ = (MainServerConfiguration)ConfigurationManager.GetSection(SECTION_NAME);
                }
                return section_;
            }
        }

        [ConfigurationProperty("instanceName", IsRequired = true)]
        public string InstanceName
        {
            get { return (string)this["instanceName"]; }
        }

        [ConfigurationProperty("maxTpSize", IsRequired = true)]
        public int MaxTpSize
        {
            get { return (int)this["maxTpSize"]; }
        }

        [ConfigurationProperty("minTpSize", IsRequired = true)]
        public int MinTpSize
        {
            get { return (int)this["minTpSize"]; }
        }

        [ConfigurationProperty("redisChannel", IsRequired = true)]
        public RedisChannelElement RedisChannel
        {
            get { return this["redisChannel"] as RedisChannelElement; }
        }

        [ConfigurationProperty("redisData", IsRequired = true)]
        public RedisDataElement RedisData
        {
            get { return this["redisData"] as RedisDataElement; }
        }
    }
}
