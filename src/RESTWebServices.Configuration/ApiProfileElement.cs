using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using TradingApi.Configuration;

namespace TradingApi.Configuration
{
    public class ApiProfileElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = false, DefaultValue = "")]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
        [ConfigurationProperty("version", IsRequired = true)]
        public string Version
        {
            get { return (string)this["version"]; }
            set { this["version"] = value; }
        }
        [ConfigurationProperty("routes", IsRequired = true)]
        public UrlMapElementCollection Routes
        {
            get { return (UrlMapElementCollection)this["routes"]; }
            set { this["routes"] = value; }
        }

        [ConfigurationProperty("dtoAssemblies", IsRequired = true)]
        public AssemblyReferenceElementCollection DtoAssemblies
        {
            get { return (AssemblyReferenceElementCollection)this["dtoAssemblies"]; }
            set { this["dtoAssemblies"] = value; }
        }

        [ConfigurationProperty("xDomainPolicy", IsRequired = true)]
        public XDomainPolicyElement XDomainPolicy
        {
            get { return (XDomainPolicyElement)this["xDomainPolicy"]; }
            set { this["xDomainPolicy"] = value; }
        }
    }
}
