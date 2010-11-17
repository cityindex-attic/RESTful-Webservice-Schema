using System.Configuration;

namespace TradingApi.Configuration
{

    [ConfigurationCollection(typeof(ApiProfileElement), AddItemName = "profile")]
    public class ApiProfileElementCollection:ConfigurationElementCollection
    {
        public ApiProfileElement this[string key]
        {
            get
            {

                return (ApiProfileElement)BaseGet(key);
            }
        }
        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ApiProfileElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for. </param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ApiProfileElement) element).Name;
        }
    }
}