using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using MetadataProcessor.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MetadataProcessor
{
    public static class JsonSchemaUtilities
    {

        


        public static JObject BuildEnumSchema(XDocument doc, Type targetType, bool includeDemoValue)
        {
            var result = BuildPropertyBase(Enum.GetUnderlyingType(targetType));
            var typeElement = GetMemberNode(doc, "T:" + targetType.FullName);

            if (includeDemoValue)
            {
                var jschemaElement = typeElement.Descendants("jschema").First();
                result.Add("demoValue", Convert.ToInt32(jschemaElement.Attribute("demoValue").Value));
            }

            ApplyDescription(result, typeElement);

            var values = new JArray();

            result.Add("enum", values);

            foreach (var value in Enum.GetValues(targetType))
            {
                values.Add(value);
            }



            var options = new JArray();
            foreach (var name in Enum.GetNames(targetType))
            {

                object value = Enum.Parse(targetType, name);


                JObject option = new JObject(new JProperty("value", value), new JProperty("label", name));
                var enumLabel = GetMemberNode(doc, "F:" + targetType.FullName + "." + name);
                if (enumLabel != null)
                {
                    var enumSummary = enumLabel.Descendants("summary").FirstOrDefault();
                    if (enumSummary != null)
                    {
                        // NOTE: this is additional to the schema 
                        option.Add(new JProperty("description", enumSummary.Value.Trim()));
                    }

                }
                options.Add(option);
            }



            result.Add("options", options);


            return result;
        }

        /// <summary>
        /// This method builds a property base, building array if necessary.
        /// If the type is not an intrinsic type, the full typename is applied. 
        /// Post-process to taste.
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static JObject BuildPropertyBase(Type targetType)
        {
            var result = new JObject();
            var jsPropertyValue = result;

            // determine if targetType is a collection type 

            // string type name comparison is a hack but the alternative is a lot more code with no added value
            if (targetType.Name.EndsWith("[]") || targetType.Name.StartsWith("List")
                || targetType.Name.StartsWith("IList") || targetType.Name.StartsWith("ICollection")
                || targetType.Name.StartsWith("IEnumerable"))
            {

                if (targetType.IsGenericType)
                {
                    targetType = targetType.GetGenericArguments()[0];
                }
                else if (targetType.IsArray)
                {
                    targetType = targetType.GetElementType();
                }

                result.Add("type", "array");

                jsPropertyValue = new JObject();
                result.Add("items", jsPropertyValue);
            }

            // get schema type

            // need to determine if the type is a system type
            if (targetType.FullName.StartsWith("System"))
            {
                var schemaType = MiscellaneousUtils.GetJsonSchemaType(targetType, Required.Always);
                jsPropertyValue.Add("type", MiscellaneousUtils.JsonSchemaReverseTypeMapping[schemaType]);
            }
            else
            {
                jsPropertyValue.Add("$ref", "#." + targetType.Name);
            }


            return result;
        }


        /// <summary>
        /// Loads the xml document from referenced type build output
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static XDocument GetXmlDocs(Type type)
        {
            string path;
            if (HttpContext.Current != null)
            {

                path = HttpContext.Current.Server.MapPath(Path.Combine("~/bin", Path.GetFileNameWithoutExtension(type.Assembly.ManifestModule.Name) + ".xml"));
            }
            else
            {
                path = Path.GetFileNameWithoutExtension(type.Assembly.ManifestModule.Name) + ".xml";
            }

            return XDocument.Load(path);
        }

        public static XElement GetMemberNode(XDocument doc, string memberName)
        {
            return doc.XPathSelectElement("/doc/members/member[@name='" + memberName + "']");
        }

        public static IEnumerable<XElement> GetMemberNodes(XDocument doc, string memberNamePartial)
        {
            return doc.XPathSelectElements("/doc/members/member[starts-with(@name,'" + memberNamePartial + "')]");
        }


        /// <summary>
        /// This will require a bit of special casing. 
        /// demoValue needs to be applied to the body of the property, including
        /// collection types. 
        /// 
        /// Will do string comparison on the type to allow for future handling
        /// of nullable union types
        /// </summary>
        public static void ApplyTypedValue(JObject propBase, XAttribute attribute)
        {
            try
            {
                var propType = propBase["type"].Value<string>();
                var propertyName = attribute.Name.ToString();
                var propertyValue = attribute.Value;
                if (propType.IndexOf("string") > -1)
                {
                    propBase.Add(propertyName, propertyValue);
                }
                if (propType.IndexOf("integer") > -1)
                {
                    propBase.Add(propertyName, Convert.ToInt64(propertyValue));

                }
                if (propType.IndexOf("number") > -1)
                {
                    // this seems right in the context of demoing the API 
                    propBase.Add(propertyName, Convert.ToDecimal(propertyValue));
                }
                if (propType.IndexOf("boolean") > -1)
                {
                    propBase.Add(propertyName, Convert.ToBoolean(propertyValue));
                }

                if (propType.IndexOf("object") > -1)
                {
                    propBase.Add(propertyName, propertyValue);
                }

                // handles only simple types - arrays of complex types are handled when the reference is resolved
                if (propType.IndexOf("array") > -1)
                {
                    var elementType = propBase["items"]["type"].Value<string>();
                    // have to deconstruct the array and build it to emit
                    // the proper JSON



                    var elements = propertyValue.Trim('[', ']').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var arrayProp = new JArray();

                    foreach (string element in elements)
                    {

                        if (elementType.IndexOf("string") > -1)
                        {
                            // strip the quotes
                            arrayProp.Add(element.Trim('"', '\''));
                        }
                        if (elementType.IndexOf("integer") > -1)
                        {
                            arrayProp.Add(Convert.ToInt64(element));

                        }
                        if (elementType.IndexOf("number") > -1)
                        {
                            arrayProp.Add(Convert.ToDecimal(element));

                        }
                        if (elementType.IndexOf("boolean") > -1)
                        {
                            arrayProp.Add(Convert.ToBoolean(element));
                        }

                    }

                    propBase.Add(propertyName, arrayProp);

                }
                if (propType.IndexOf("any") > -1)
                {
                    propBase.Add(propertyName, propertyValue);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(
                    string.Format("An exception occured when applying attribute\n\n{1}\n\nto\n\n{0}", propBase, attribute),
                    e);
            }
        }

        public static void ApplyDescription(JObject propBase, XElement propElement)
        {
            var summaryElement = propElement.Descendants("summary").FirstOrDefault();
            if (summaryElement != null)
            {
                string summary = summaryElement.Value;
                summary = summary.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\t", " ");
                summary = Regex.Replace(summary, "\\s+", " ", RegexOptions.Singleline);
                propBase.Add("description", summary.Trim());
            }
        }

        public static void ApplyPropertyAttribute(JObject propBase, XAttribute attribute,string parentName,string name)
        {
            
            
            
       

            string attributeName = attribute.Name.ToString();
            switch (attributeName)
            {
                // handle type agnostic attributes first

                // boolean
                case "optional":
                case "additionalProperties":
                case "uniqueItems":
                case "minimumCanEqual":
                case "maximumCanEqual":
                    propBase.Add(attributeName, Convert.ToBoolean(attribute.Value));
                    break;
                // string
                case "pattern":
                case "title":
                case "description":
                case "format":
                case "contentEncoding":
                case "extends":
                    propBase.Add(attributeName, attribute.Value);
                    break;

                // numeric values
                case "minimum":
                case "maximum":

                    var propType = propBase["type"];
                    string propTypeString = null;
                    if (propType != null)
                    {
                        propTypeString = propType.Value<string>();
                    }

                    if (propTypeString == "array")
                    {
                        propTypeString = propBase["items"]["type"].Value<string>();

                        // if the type is not an object, make it an object so that we can add the extra properties
                    }


                    // will be either number or integer
                    if (propTypeString == "number")
                    {
                        propBase.Add(attributeName, Convert.ToDecimal(attribute.Value));
                    }
                    else if (propTypeString == "integer")
                    {
                        propBase.Add(attributeName, Convert.ToInt64(attribute.Value));
                    }
                    else
                    {
                        throw new InvalidOperationException("invalid property type for attribute min/max\n"+string.Format("Type:{0}, Member:{1}, Attribute:{2}",parentName, name, attributeName));
                    }
                    break;
                case "minItems":
                case "maxItems":
                case "maxLength":
                case "minLength":
                    propBase.Add(attributeName, Convert.ToInt64(attribute.Value));
                    break;
                case "default":
                    ApplyTypedValue(propBase, attribute);
                    break;
                case "enum":
                    throw new NotImplementedException("enum is constructed by use of underlying type attributeName\n" + string.Format("Type:{0}, Member:{1}, Attribute:{2}", parentName, name, attributeName));


                case "requires":
                case "divisibleBy":
                case "disallow":
                    throw new NotImplementedException(attributeName + " is not supported\n" + string.Format("Type:{0}, Member:{1}, Attribute:{2}", parentName, name, attributeName));
                default:
                    // ignore all of the rest
                    break;
            }
        }


        public static JObject BuildTypeSchema(Type type, XDocument doc, bool includeDemoValue)
        {
            var typeElement = GetMemberNode(doc, "T:" + type.FullName);
            if (typeElement != null)
            {
                var typeJschemaElement = typeElement.Descendants("jschema").FirstOrDefault();
                // no jschema on the type, no process
                if (typeJschemaElement != null)
                {


                    JObject jsob;
                    if (type.IsEnum)
                    {
                        jsob = BuildEnumSchema(doc, type, includeDemoValue);
                    }
                    else
                    {
                        jsob = new JObject();
                        jsob.Add("id", type.Name);
                        jsob.Add("type", "object");
                        ApplyDescription(jsob, typeElement);
                        // check for inheritance
                        if (type.BaseType != null && type.BaseType.IsClass && type.BaseType != typeof(object))
                        {
                            // must be derived from one of our classes
                            JObject propObj = new JObject(new JProperty("$ref", "#." + type.BaseType.Name));
                            // TODO: go ahead and add description to reference. we are the only people in the world that are
                            // excercising json-schema and smd to this extent. let them follow us.

                            jsob.Add(new JProperty("extends", propObj));
                        }

                        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
                        var schemaProperties = new JObject();
                        jsob.Add("properties", schemaProperties);

                        foreach (PropertyInfo property in properties)
                        {

                            XElement propElement = GetMemberNode(doc, "P:" + type.FullName + "." + property.Name);
                            if (propElement != null)
                            {
                                XElement metaElement = propElement.Descendants("jschema").FirstOrDefault();
                                if (metaElement != null)
                                {
                                    BuildPropertySchema(propElement, metaElement, includeDemoValue, schemaProperties, property.Name, property.PropertyType, type.FullName);
                                }

                            }


                        }

                    }



                    return jsob;
                }
            }
            return null;
        }


        public static JObject BuildPropertySchema(XElement docElement, XElement metaElement, bool includeDemoValue, JObject metaContainer, string propertyName, Type propertyType,string parentName)
        {
            try
            {
                // no jschema, no process

                var underlyingType = metaElement.Attributes("underlyingType").FirstOrDefault();

                if (underlyingType != null)
                {
                    propertyType = Type.GetType(underlyingType.Value, true, false);
                }


                var propBase = BuildPropertyBase(propertyType);


                metaContainer.Add(propertyName, propBase);

                ApplyDescription(propBase, docElement);

                if (includeDemoValue)
                {
                    var demoValueAttribute = metaElement.Attributes("demoValue").FirstOrDefault();

                    if (demoValueAttribute != null)
                    {
                        ApplyTypedValue(propBase, demoValueAttribute);
                    }
                }


                JObject attributeTarget = propBase;
                if (propBase["type"] != null && propBase["type"].Value<string>() == "array")
                {
                    attributeTarget = propBase["items"].Value<JObject>();
                }

                foreach (var attribute in metaElement.Attributes())
                {
                    ApplyPropertyAttribute(attributeTarget, attribute, parentName,propertyName);
                }
                if (propBase["optional"] == null)
                {
                    propBase.Add("optional", false);
                }
                return propBase;
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Exception with\n\n{0}",docElement),e);
            }
        }


        

        
    }
}