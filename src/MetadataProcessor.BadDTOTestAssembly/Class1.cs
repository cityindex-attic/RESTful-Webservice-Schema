using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataProcessor.BadDTOTestAssembly
{
    /// <summary>
    /// 
    /// </summary>
    /// <jschema/>
    public class ClassWithBadCollections
    {
        /// <summary>
        /// 
        /// </summary>
        /// <jschema/>
        public List<Class1> ListProperty { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <jschema/>
    public class Class1
    {
        
    }
}
