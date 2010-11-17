using System.Configuration;

namespace TradingApi.Configuration
{
    [ConfigurationCollection(typeof(AssemblyReferenceElement))]
    public class AssemblyReferenceElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AssemblyReferenceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AssemblyReferenceElement)element).Assembly;
        }
    }

    public class AssemblyReferenceElement : ConfigurationElement
    {
        [ConfigurationProperty("assembly", IsKey = true, IsRequired = true)]
        public string Assembly
        {
            get
            {
                return (string) this["assembly"];
            }
            set
            {
                this["assembly"] = value;
            }
        }
    }
}