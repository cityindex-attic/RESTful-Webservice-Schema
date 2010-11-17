using System.Configuration;

namespace TradingApi.Configuration
{
    [ConfigurationCollection(typeof(UrlMapElement))]
    public class UrlMapElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new UrlMapElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((UrlMapElement)element).Name;
        }
    }
}