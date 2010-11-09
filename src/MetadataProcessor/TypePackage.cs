using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MetadataProcessor.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace MetadataProcessor
{
    /// <summary>
    /// This code generates expected schema, for the target code, but was written on the fly
    /// and requires a couple more hours refactoring before it is suitable for humans.
    /// </summary>
    public class TypePackage
    {
        public Type Type { get; set; }
        public JObject Schema { get; set; }
        public XDocument XmlDocs { get; set; }
        public string SchemaPrefix { get; set; }

        public TypePackage(Type type, string schemaPrefix)
        {
            SchemaPrefix = schemaPrefix;
            Type = type;
            XmlDocs = XDocument.Load(Path.GetFileNameWithoutExtension(Type.Assembly.ManifestModule.Name) + ".xml");

            if (type.IsEnum)
            {
                BuildEnumSchema();
                return;
            }

            // fall through to default build strategy
            BuildSchema();
        }
        private bool IsBaseExtensible()
        {
            return Type.BaseType != null && Type.BaseType.IsClass && Type.BaseType != typeof(object);
        }
        private static void SetBoolProperty(JObject target, string propertyName, bool? value)
        {
            if (value.HasValue)
            {
                target.Add(new JProperty(propertyName, value));
            }
        }
        private static void SetIntProperty(JObject target, string propertyName, int? value)
        {
            if (value.HasValue)
            {
                target.Add(new JProperty(propertyName, value));
            }
        }

        private static void SetStringProperty(JObject target, string propertyName, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                target.Add(new JProperty(propertyName, value));
            }
        }
        private static int? GetIntValue(string value)
        {
            return string.IsNullOrEmpty(value) ? default(int?) : Convert.ToInt32(value);
        }
        private static bool? GetBoolValue(string value)
        {
            return string.IsNullOrEmpty(value) ? default(bool?) : Convert.ToBoolean(value);
        }
        private static void SetTypedProperty(JObject jschemaProperty, string propertyJSType, string stringValue, string propertyName)
        {
            if (!string.IsNullOrEmpty(stringValue))
            {
                object value;
                switch (propertyJSType)
                {
                    case "null":
                    case "array":
                    case "any":
                    case "object":
                    case "string":
                        value = (string)stringValue;
                        break;
                    case "integer":
                        value = Convert.ToInt64(stringValue);
                        break;
                    case "number":
                        value = Convert.ToDouble(stringValue);
                        break;
                    case "boolean":
                        value = Convert.ToBoolean(stringValue);
                        break;
                    default:
                        value = (string)stringValue;
                        break;
                        //throw new NotSupportedException(string.Format("{0} value for type {1} is not supported", propertyName, propertyJSType));
                }

                jschemaProperty.Add(new JProperty(propertyName, value));
            }
        }


        private void BuildSchema()
        {
            Schema = new JObject();
            var typenode = GetTypeNode();
            var summary = GetNodeValue(typenode, "summary");
            var jsTypeNode = typenode.Descendants("jschema").FirstOrDefault();

            // {
            //     "id": "ErrorResponseDTO",
            Schema.Add(new JProperty("id", Type.Name));
            if (!string.IsNullOrEmpty(summary))
            {
                Schema.Add(new JProperty("description", summary));
            }
            //     "description": "foo desc",
            //     "type": "object",
            Schema.Add(new JProperty("type", "object"));

            if (IsBaseExtensible())
            {
                Schema.Add(new JProperty("extends", new JObject(new JProperty("$ref", SchemaPrefix + Type.BaseType.Name))));
            }
            var properties = new JObject();

            //     "properties": {
            Schema.Add(new JProperty("properties", properties));
            var propertyNodes = GetNodes("P");
            var publicInstanceProperties = Type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList();
            foreach (PropertyInfo publicInstanceProperty in publicInstanceProperties)
            {

                var jschemaProperty = new JObject();

                Boolean? jsOptional = null;


                // NOTE: nullable union types are not yet supported



                // get xml doc node, if any
                var propertyNode = propertyNodes.FirstOrDefault(n => n.Attribute("name").Value == "P:" + Type.FullName + "." + publicInstanceProperty.Name);

                if (propertyNode != null)
                {
                    SetStringProperty(jschemaProperty, "description", GetNodeValue(propertyNode, "summary"));

                    var jsPropNode = propertyNode.Descendants("jschema").FirstOrDefault();

                    if (jsPropNode != null)
                    {
                        SetStringProperty(jschemaProperty, "title", GetAttributeValue(jsPropNode, "title"));
                        SetStringProperty(jschemaProperty, "format", GetAttributeValue(jsPropNode, "format"));
                        SetStringProperty(jschemaProperty, "contentEncoding", GetAttributeValue(jsPropNode, "contentEncoding"));
                        SetStringProperty(jschemaProperty, "pattern", GetAttributeValue(jsPropNode, "pattern"));

                        SetIntProperty(jschemaProperty, "minLength", GetIntValue(GetAttributeValue(jsPropNode, "minLength")));
                        SetIntProperty(jschemaProperty, "maxLength", GetIntValue(GetAttributeValue(jsPropNode, "maxLength")));
                        SetBoolProperty(jschemaProperty, "maximumCanEqual", GetBoolValue(GetAttributeValue(jsPropNode, "maximumCanEqual")));
                        SetBoolProperty(jschemaProperty, "minimumCanEqual", GetBoolValue(GetAttributeValue(jsPropNode, "minimumCanEqual")));

                        // 
                        jsOptional = GetBoolValue(GetAttributeValue(jsPropNode, "optional")); // needed elsewhere
                        SetBoolProperty(jschemaProperty, "optional", jsOptional);
                        var jsRequired = jsOptional.GetValueOrDefault() ? Required.AllowNull : Required.Always;
                        // determine the property type

                        // if an explicit underlying type, simply reference it and move on
                        var propertyTypeNode = jsPropNode.Attributes("underlyingType").FirstOrDefault();
                        if (propertyTypeNode != null)
                        {
                            // RISK: the assembly must be loaded meaning that the property type should be in the same assembly as the containing type
                            // TODO: really necessary? are we comfortable with arbitrary value here? if so previous constraint is relaxed
                            // adding a fully qualified typename may alleviate this constraint
                            var propertyType = Type.GetType(propertyTypeNode.Value, true, false);
                            jschemaProperty.Add(new JProperty("type", new JObject(new JProperty("$ref", SchemaPrefix + propertyType.Name))));

                        }
                        else
                        {
                            // FIXME: handle union types


                            string propertyJSType;
                            JsonSchemaType jsonSchemaType = MiscellaneousUtils.GetJsonSchemaType(publicInstanceProperty.PropertyType, jsRequired);

                            propertyJSType = MiscellaneousUtils.JsonSchemaReverseTypeMapping[jsonSchemaType];
                            jschemaProperty.Add(new JProperty("type", propertyJSType));





                            SetTypedProperty(jschemaProperty, propertyJSType, GetAttributeValue(jsPropNode, "demoValue"), "demoValue");
                            SetTypedProperty(jschemaProperty, propertyJSType, GetAttributeValue(jsPropNode, "default"), "default");
                            SetTypedProperty(jschemaProperty, propertyJSType, GetAttributeValue(jsPropNode, "minimum"), "minimum");
                            SetTypedProperty(jschemaProperty, propertyJSType, GetAttributeValue(jsPropNode, "maximum"), "maximum");
                        }
                    }
                }
                else
                {
                    // no xml docs, go with what we have
                    var propertyJSType = MiscellaneousUtils.JsonSchemaReverseTypeMapping[MiscellaneousUtils.GetJsonSchemaType(publicInstanceProperty.PropertyType, Required.Always)];
                    jschemaProperty.Add(new JProperty("type", propertyJSType));
                }

                // do some post processing for list/array types - 
                // string type name comparison is a hack but the alternative is a lot more code

                string propertyName = publicInstanceProperty.PropertyType.Name;
                if (propertyName.EndsWith("[]") || propertyName.StartsWith("List") || propertyName.StartsWith("IList") || propertyName.StartsWith("ICollection") || propertyName.StartsWith("IEnumerable"))
                {

                    JObject arrayProp = new JObject(new JProperty("type", "array"), new JProperty("items", jschemaProperty));

                    // get the underlying type and apply to jschemaProperty as it will have been reported as string earlier

                    var itemsType = publicInstanceProperty.PropertyType;
                    if (itemsType.IsArray)
                    {
                        itemsType = itemsType.GetElementType();
                    }
                    else if (itemsType.IsGenericType)
                    {
                        itemsType = itemsType.GetGenericArguments()[0];
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }

                    if (itemsType.IsClass && itemsType != typeof(object))
                    {
                        jschemaProperty.Property("type").Value = new JObject(new JProperty("$ref", SchemaPrefix + itemsType.Name));
                    }
                    else
                    {
                        var itemsSchemaType = MiscellaneousUtils.GetJsonSchemaType(itemsType, Required.Always);
                        jschemaProperty.Property("type").Value = MiscellaneousUtils.JsonSchemaReverseTypeMapping[itemsSchemaType];
                    }

                    // remove demovalue, if any from the jschemaProperty and apply to array prop

                    properties.Add(new JProperty(publicInstanceProperty.Name, arrayProp));
                }
                else
                {
                    properties.Add(new JProperty(publicInstanceProperty.Name, jschemaProperty));
                }


            }

        }
        private string GetAttributeValue(XElement node, string attributeName)
        {
            XAttribute attribute = node.Attributes(attributeName).FirstOrDefault();
            return attribute != null ? attribute.Value : null;
        }

        private XElement GetTypeNode()
        {
            var node = XmlDocs.Descendants("members").Descendants("member").Where(n => n.Attribute("name").Value == "T:" + Type.FullName).FirstOrDefault();
            return node;
        }
        private List<XElement> GetNodes(string prefix)
        {
            List<XElement> nodes = XmlDocs.Descendants("members").Descendants("member").Where(n => n.Attribute("name").Value.StartsWith(prefix + ":" + Type.FullName)).ToList();
            return nodes;
        }


        private static string GetNodeValue(XElement node, string childName)
        {
            var child = node.Descendants(childName).FirstOrDefault();
            return child != null ? child.Value.Trim() : null;
        }


        #region Enum
        /// <summary>
        /// simple enum - int based, no flags
        /// </summary>
        private void BuildEnumSchema()
        {

            var typeNode = GetTypeNode();

            var fieldNodes = GetNodes("F");

            Schema = new JObject();

            Schema.Add(new JProperty("id", Type.Name));
            var summary = GetNodeValue(typeNode, "summary");
            if (!string.IsNullOrEmpty(summary))
            {
                Schema.Add(new JProperty("description", summary));
            }
            Schema.Add(new JProperty("type", "integer"));
            Schema.Add(new JProperty("enum", new JArray(Enum.GetValues(Type))));
            Schema.Add(new JProperty("options", new JArray(Enum.GetValues(Type).Cast<int>().Select(v => GetOptionsObject(Type, v, fieldNodes)).ToArray())));
        }

        private JObject GetOptionsObject(Type type, int v, List<XElement> fieldNodes)
        {
            JObject newJObject = new JObject(new JProperty("value", v), new JProperty("label", Enum.GetName(Type, v)));

            var summary = GetFieldSummary(type, v, fieldNodes);
            if (!string.IsNullOrEmpty(summary))
            {
                newJObject.Add(new JProperty("description", summary));
            }
            return newJObject;
        }



        private string GetFieldSummary(Type type, int v, IEnumerable<XElement> fieldNodes)
        {
            var x = fieldNodes.Select(n => n.Attribute("name").Value).ToArray();
            var name = "F:" + type.FullName + "." + Enum.GetName(Type, v);
            var node = fieldNodes.FirstOrDefault(n => n.Attribute("name").Value == name);
            if (node != null)
            {
                var summary = node.Descendants("summary").FirstOrDefault();
                if (summary != null)
                {
                    return summary.Value.Trim();
                }

            }
            return null;
        }
        #endregion



    }
}