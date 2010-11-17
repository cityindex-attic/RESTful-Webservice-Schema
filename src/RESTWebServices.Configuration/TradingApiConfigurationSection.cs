using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using TradingApi.Configuration;

namespace TradingApi.Configuration
{
    public class TradingApiConfigurationSection : ConfigurationSection
    {
        private static TradingApiConfigurationSection _instance;

        /// <summary>
        /// Static convenience accessor. Assumes that configuration is readonly at runtime.
        /// </summary>
        public static TradingApiConfigurationSection Instance
        {
            get
            {
                // not worried about thread safety here. A collision can only
                // happen at the startup of the appdomain and the worse that 
                // can happen is that the static (readonly) instance is assigned
                // twice
                if (_instance == null)
                {
                    _instance = (TradingApiConfigurationSection)ConfigurationManager.GetSection("tradingApi");
                }
                return _instance;
            }
        }

        //[ConfigurationProperty("activeMQHandlers")]
        //public ActiveMQRequestConfigurationElementCollection ActiveMQHandlers
        //{
        //    get
        //    {
        //        return (ActiveMQRequestConfigurationElementCollection)this["activeMQHandlers"];
        //    }
        //}

        [ConfigurationProperty("profiles")]
        public ApiProfileElementCollection Profiles
        {
            get
            {
                return (ApiProfileElementCollection)this["profiles"];
            }
        }


    }
}
