using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace TradingApi.Configuration
{
   
    public class XDomainPolicyElement : ConfigurationElement
    {
        [ConfigurationProperty("allowAccessFrom", IsKey = true, IsRequired = true)]
        public string AllowAccessFrom
        {
            get { return (string)this["allowAccessFrom"]; }
            set { this["allowAccessFrom"] = value; }
        }
    }
}
