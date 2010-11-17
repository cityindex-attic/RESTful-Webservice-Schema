using System.Configuration;

namespace TradingApi.Configuration
{
    public class UrlMapElement : ConfigurationElement
    {

        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }


        /// <summary>
        /// The endpoint portion of the path
        /// </summary>
        [ConfigurationProperty("endpoint", IsKey = true, IsRequired = true)]
        public string Endpoint
        {
            get { return (string)this["endpoint"]; }
            set { this["endpoint"] = value; }
        }

        /// <summary>
        /// the method and pattern portion of the path
        /// </summary>
        [ConfigurationProperty("urlPattern")]
        public string UrlPattern
        {
            get { return (string)this["urlPattern"]; }
            set { this["urlPattern"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("filePath")]
        public string FilePath
        {
            get { return (string)this["filePath"]; }
            set { this["filePath"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("pathInfo")]
        public string PathInfo
        {
            get { return (string)this["pathInfo"]; }
            set { this["pathInfo"] = value; }
        }

        /// <summary>
        /// The service handler type
        /// </summary>
        [ConfigurationProperty("type")]
        public string Type
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }
    }
}