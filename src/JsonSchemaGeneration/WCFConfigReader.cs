using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using TradingApi.Configuration;

namespace JsonSchemaGeneration
{
    public class WcfConfig
    {
        public List<UrlMapElement> Routes;
        public string[] DTOAssemblyNames;
    }

    public class WcfConfigReader
    {
        public WcfConfig Read(string configPath)
        {
            var wcfConfig = new WcfConfig();
            var config = XDocument.Load(configPath);
            var apiNode = config.XPathSelectElement("configuration/tradingApi");
            var profile = apiNode.XPathSelectElement("profiles").Descendants("profile").First();
            wcfConfig.DTOAssemblyNames = profile.Descendants("dtoAssemblies").Descendants("add").Select(n => n.Attribute("assembly").Value).ToArray();

            var routeNodes = profile.XPathSelectElement("routes").XPathSelectElements("add").ToList();
            wcfConfig.Routes = new List<UrlMapElement>();
            foreach (var item in routeNodes)
            {
                UrlMapElement map = new UrlMapElement()
                {
                    Endpoint = item.Attribute("endpoint").Value /*+ item.Attribute("pathInfo").Value*/,
                    Name = item.Attribute("name").Value,
                    Type = item.Attribute("type").Value
                };
                wcfConfig.Routes.Add(map);
            }

            return wcfConfig;
        }
    }
}
