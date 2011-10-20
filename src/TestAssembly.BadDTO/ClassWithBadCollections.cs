using System.Collections.Generic;

namespace MetadataProcessor.BadDTOTestAssembly
{
    /// <summary>
    /// We only support metadata generation of IList.
    /// This class contains a set of collection types that are not supported
    /// </summary>
    /// <jschema/>
    public class ClassWithBadCollections
    {
        /// <summary>
        /// This should throw an exception
        /// </summary>
        /// <jschema/>
        public List<Class1> ListProperty { get; set; }
        /// <summary>
        /// This should throw an exception
        /// </summary>
        /// <jschema/>
        public IEnumerable<Class1> IEnumerableProperty { get; set; }
    }
}