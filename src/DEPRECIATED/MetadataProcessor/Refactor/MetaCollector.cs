using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace MetadataProcessor
{
    /// <summary>
    /// This class simply collects all relevant meta data from a type
    /// 
    /// </summary>
    public static class MetaCollector
    {

        /// <summary>
        /// A type will be either a ServiceType or a DataType (or null)
        /// Currently the only meta being collected from a type is the summary. 
        /// Should probably resolve a convention wherein the group is defined and described by the service type/interface.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>null if not to be processed</returns>  
        public static MetaDataPackage GetMeta(this Type type)
        {
            var package = new MetaDataPackage();

            var docs = XmlDocExtensions.GetXmlDocs(type);
            var memberNode = XmlDocExtensions.GetMemberNode(docs, "T:" + type.FullName);


            XElement metaNode;

            // determine package type and get meta node
            if (memberNode.Descendants("jschema").Count() > 0)
            {
                package.PackageType = MetaDataPackageType.DataType;
                metaNode = memberNode.Descendants("jschema").First();
            }
            else if (memberNode.Descendants("smd").Count() > 0)
            {
                package.PackageType = MetaDataPackageType.ServiceMethod;
                metaNode = memberNode.Descendants("smd").First();
            }
            else
            {
                // no smd or jschema, return null
                return null;
            }

            GetXmlAttributes(package, memberNode, metaNode);

            switch (package.PackageType)
            {
                case MetaDataPackageType.ServiceType:
                    
                    break;
                case MetaDataPackageType.DataType:
                    // get properties

                    break;
                default:
                    throw new InvalidOperationException();
            }





            return package;
        }

        private static MetaDataPackage GetMeta(this PropertyInfo info)
        {
            MetaDataPackage package = GetMetaDataPackage(info);
            return package;
        }

        private static MetaDataPackage GetMeta(this MethodInfo info)
        {
            MetaDataPackage package = GetMetaDataPackage(info);
            return package;
        }

        /// <summary>
        /// This is the method that actually collects and packages the raw meta data. The process
        /// is the same regardless of the member. It is the post processing that is different.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private static MetaDataPackage GetMetaDataPackage(MemberInfo info)
        {

            // get xml docs
            var docs = XmlDocExtensions.GetXmlDocs(info.DeclaringType);
            // get custom attributes
            var attributes = info.GetCustomAttributes(true);


            var package = new MetaDataPackage();

            return package;
        }

        private static void GetXmlAttributes(MetaDataPackage package, XElement memberNode, XElement metaNode)
        {
            package.DocumentationMembers["description"] = XmlDocExtensions.GetNodeValue(memberNode, "summary");
            XmlDocExtensions.GetAttributeValues(metaNode, package.DocumentationMembers);
        }

    }
}
