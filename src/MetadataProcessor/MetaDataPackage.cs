using System;
using System.Collections.Generic;

namespace MetadataProcessor
{
    public class MetaDataPackage
    {
        public MetaDataPackage()
        {
            DocumentationMembers = new Dictionary<string, object>();
            CustomAttributes = new List<Attribute>();
            Packages=new List<MetaDataPackage>();
        }

        /// <summary>
        /// Contains aggregation of standard xml doc comments and smd/jschema comment attributes.
        /// Initially these values are string but will be converted, if necessary, to appropriate 
        /// types in secondary steps
        /// </summary>
        public Dictionary<string, object> DocumentationMembers { get; set; }

        /// <summary>
        /// All attributes that may be decorating the type/property/method
        /// </summary>
        public List<Attribute> CustomAttributes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MetaDataPackageType PackageType { get; set; }

        /// <summary>
        /// Subordinate packages (properties, methods, parameters)
        /// </summary>
        public List<MetaDataPackage> Packages { get; set; }
    }
}