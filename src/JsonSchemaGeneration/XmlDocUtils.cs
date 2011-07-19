using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;

namespace JsonSchemaGeneration
{
    public static class XmlDocUtils
    {
        public static XElement GetXmlDocTypeNodeWithJSchema(this Type type)
        {
            var typeNode = type.GetXmlDocTypeNode();

            if (typeNode != null)
            {
                if (typeNode.XPathSelectElement("jschema") == null)
                {
                    typeNode = null;
                }
            }

            return typeNode;
        }
        public static XElement GetXmlDocTypeNodeWithSMD(this Type type)
        {
            XDocument doc = type.GetXmlDocs();
            var node = doc.XPathSelectElement("/doc/members/member[@name = 'T:" + type.FullName + "']");
            if (node != null)
            {
                if (node.XPathSelectElement("smd") == null)
                {
                    node = null;
                }
            }

            return node;
        }
        public static XElement GetXmlDocMemberNodeWithSMD(this Type type, string name)
        {
            XElement node2 = null;
            XDocument doc = type.GetXmlDocs();

            if (doc != null)
            {

                node2 = doc.XPathSelectElement("/doc/members/member[@name='M:" + name + "']") ?? doc.XPathSelectElement("/doc/members/member[starts-with(@name,'M:" + name + "(')]");

                if (node2 != null)
                {
                    if (node2.XPathSelectElement("smd") == null)
                    {
                        node2 = null;
                    }
                }
            }
            return node2;
        }
        public static XElement GetXmlDocNodeJschema(this Type type, string typeName, string memberName)
        {
            XDocument doc = type.GetXmlDocs();
            var node = doc.XPathSelectElement("/doc/members/member[@name = '" + typeName + ":" + memberName + "']");
            if (node != null)
            {
                if (node.XPathSelectElement("jschema") == null)
                {
                    node = null;
                }
            }

            return node;
        }

        public static XElement GetXmlDocTypeNode(this Type type)
        {
            return type.GetXmlDocNodeJschema("T", type.FullName);
        }


        public static XElement GetXmlDocMemberNode(this Type type, string name)
        {
            return type.GetXmlDocNodeJschema("M", type.FullName + "." + name);
        }

        public static XElement GetXmlDocFieldNode(this Type type, string name)
        {
            return type.GetXmlDocNodeJschema("F", type.FullName + "." + name);
        }

        public static XElement GetXmlDocPropertyNode(this Type type, string name)
        {
            return type.GetXmlDocNodeJschema("P", type.FullName + "." + name);
        }
        public static void EnsureXmlDocsAreValid(params string[] assemblyNames)
        {
            foreach (var assembly in UtilityExtensions.GetAssemblies(assemblyNames))
            {
                foreach (Type type in assembly.GetTypes())
                {
                    var doc = type.GetXmlDocs();
                    doc.EnsureXmlDocsValid();
                }
            }

        }

        /// <summary>
        /// Loads the xml document from referenced type build output
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static XDocument GetXmlDocs(this Type type)
        {
            string fileName = Path.GetFileNameWithoutExtension(type.Assembly.ManifestModule.Name) + ".xml";

            string filePath = HttpContext.Current != null
                                  ? HttpContext.Current.Server.MapPath(Path.Combine("~/bin", fileName))
                                  : fileName;

            try
            {
                XDocument doc = XDocument.Load(filePath);
                return doc;
            }
            catch
            {

                throw;
            }

        }

        public static void EnsureXmlDocsValid(this XDocument doc)
        {
            var badCommentRX = new Regex("<!-- Badly formed XML comment ignored for member (?<bad>\".*\") -->", RegexOptions.ExplicitCapture);
            var matches = badCommentRX.Matches(doc.ToString());
            if (matches.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Match m in matches)
                {
                    sb.Append(m.Groups["bad"].Value + ", ");
                }
                throw new Exception("Badly formed XML comments for " + sb);
            }
        }
    }
}