using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MetadataProcessor
{
    public class XmlDocExtensions
    {

        /// <summary>
        /// Loads the xml document from referenced type build output
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static XDocument GetXmlDocs(Type type)
        {
            string fileName = Path.GetFileNameWithoutExtension(type.Assembly.ManifestModule.Name) + ".xml";

            string filePath = HttpContext.Current != null
                                  ? HttpContext.Current.Server.MapPath(Path.Combine("~/bin", fileName))
                                  : fileName;

            return XDocument.Load(filePath);
        }

        public static XElement GetMemberNode(XDocument doc, string memberName)
        {
            return doc.XPathSelectElement("/doc/members/member[@name='" + memberName + "']");
        }

        public static string FixWhitespace(string value)
        {
            value = value.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\t", " ");
            value = Regex.Replace(value, "\\s+", " ", RegexOptions.Singleline);
            return value;
        }

        public static string GetNodeValue(XElement memberNode, string nodeName)
        {
            var node = memberNode.Descendants(nodeName).FirstOrDefault();
            string nodeValue = node != null ? XmlDocExtensions.FixWhitespace(node.Value) : "";
            return nodeValue;
        }

        public static void GetAttributeValues(XElement memberNode, Dictionary<string, object> values)
        {
            foreach (var attribute in memberNode.Attributes())
            {
                values[attribute.Name.ToString()] = attribute.Value;
            }
        }

    }
}