using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using TradingApi.ServerCommon;

namespace TradingApi.ActiveMQ.Configuration
{
    public class ActiveMQRequestConfigurationElement : ConfigurationElement, IActiveMQConfig
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("handlerType")]
        public HandlerType Type
        {
            get { return (HandlerType)this["handlerType"]; }
            set { this["handlerType"] = value; }
        }

        [ConfigurationProperty("uri")]
        public string Uri
        {
            get { return (string)this["uri"]; }
            set { this["uri"] = value; }
        }

        [ConfigurationProperty("queue")]
        public string Queue
        {
            get { return (string)this["queue"]; }
            set { this["queue"] = value; }
        }
    }
}
