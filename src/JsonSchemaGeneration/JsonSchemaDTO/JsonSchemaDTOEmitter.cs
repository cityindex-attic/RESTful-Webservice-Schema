using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSchemaGeneration.JsonSchemaDTO
{
    public class JsonSchemaDtoEmitter
    {
            

        public string EmitDtoJson(params string[] assemblyNames)
        {
            var schemaObj = new JObject();
            var assemblies = UtilityExtensions.GetAssemblies(assemblyNames);
            


            var schemaProperties = new JObject();
            schemaObj["properties"] = schemaProperties;

            var types = UtilityExtensions.GetSchemaTypes(assemblies);

            foreach (Type type in types)
            {
                var typeNode = type.GetXmlDocTypeNodeWithJSchema();
                var jSchemaNode = typeNode.XPathSelectElement("jschema");

                var typeObj = new JObject();
                typeObj["id"] = type.Name;

                if (type.IsEnum)
                {
                    RenderEnum(type, typeObj);
                }
                else if (type.IsClass)
                {
                    RenderType(type, typeObj);
                }
                else
                {
                    throw new NotSupportedException(type.Name + " is not supported ");
                }

                ApplyDescription(typeObj, typeNode);

                AppendDemoValueAttribute(typeObj, jSchemaNode);

                schemaProperties.Add(type.Name, typeObj);
                
            }

            return schemaObj.ToString();

        }

        private static void AppendDemoValueAttribute(JObject typeObj, XElement jSchemaNode)
        {

            var node = jSchemaNode.Attribute("demoValue");
            if (node != null)
            {
                typeObj["demoValue"] = node.Value;
            }
        }
        
        private static void RenderEnum(Type type, JObject typeObj)
        {
            typeObj["type"] = "integer";

            var enumArray = new JArray();
            typeObj["enum"] = enumArray;
            var optionsArray = new JArray();
            typeObj["options"] = optionsArray;
            // OK we have a quandry - there are service endpoint implementations
            // that will accept the enum 'name' as a string rather than it's numeric value
            // so need to enable that capacity via a new proprietary jschema attribute
            // but by default, render the enum values as numeric

            var enumValues = Enum.GetValues(type);
            foreach (int enumValue in enumValues)
            {
                enumArray.Add(enumValue);
                var option = new JObject();
                option["value"] = enumValue;
                string fieldName = Enum.GetName(type, enumValue);
                option["label"] = fieldName;

                string description = "";

                var fieldNode = type.GetXmlDocFieldNode(fieldName);
                if (fieldNode != null)
                {
                    // TODO: may have to do some re-encoding
                    description = fieldNode.Value;
                }

                option["description"] = description;
                optionsArray.Add(option);
            }
        }
        private static void SetAttribute(JObject pobj, XElement jsNode, string attributeName)
        {
            var node = jsNode.Attribute(attributeName);
            if (node != null)
            {
                var nodeValue = node.Value;
                pobj[attributeName] = nodeValue;
            }
        }
        private static string MungeAttributeName(string name)
        {
            if (name == "minimum")
            {
                name = "minValue";
            }
            if (name == "maximum")
            {
                name = "maxValue";
            }
            return name;
        }
        private static void RenderType(Type type, JObject typeObj)
        {
            string typeName = type.Name;

            typeObj["type"] = "object";

            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                typeObj["extends"] = "#/" + type.BaseType.Name;
            }


            var properties = new JObject();
            typeObj["properties"] = properties;

            // TODO: should we render fields if they are jschema decorated?
            // seems like some fields are getting scattered about in the DTO classes

            foreach (var propertyInfo in type.GetProperties())
            {
                string memberName = propertyInfo.Name;
                var pnode = type.GetXmlDocPropertyNode(memberName);
                if (pnode != null)
                {
                    var pobj = new JObject();
                    properties[memberName] = pobj;
                    var jsNode = pnode.XPathSelectElement("jschema");
                    RenderTypeMeta(pobj, propertyInfo.PropertyType);

                    foreach (var item in jsNode.Attributes())
                    {
                        string name = item.Name.ToString();
                        string value = item.Value;
                        ApplyPropertyAttribute(pobj, value, typeName, name);
                    }
                    ApplyDescription(pobj, pnode);
                }
            }

            // do fields too - api team lets these slip by
            foreach (var fieldInfo in type.GetFields())
            {
                string memberName = fieldInfo.Name;
                var pnode = type.GetXmlDocPropertyNode(memberName);
                if (pnode != null)
                {
                    // TODO: should we emit a warning?

                    var pobj = new JObject();
                    properties[memberName] = pobj;
                    var jsNode = pnode.XPathSelectElement("jschema");
                    RenderTypeMeta(pobj, fieldInfo.FieldType);

                    foreach (var item in jsNode.Attributes())
                    {
                        string name = item.Name.ToString();
                        string value = item.Value;
                        ApplyPropertyAttribute(pobj, value, typeName, name);
                    }
                    ApplyDescription(pobj, pnode);

                }

            }

        }
        private static JObject GetSchemaType(Type type)
        {
            TypeCode typecode = Type.GetTypeCode(type);
            var typeObj = new JObject();
            switch (typecode)
            {
                case TypeCode.Boolean:
                    typeObj["type"] = "boolean";
                    break;
                case TypeCode.Byte:
                    typeObj["type"] = "integer";
                    typeObj["format"] = "byte";
                    typeObj["minValue"] = 0;
                    typeObj["maxValue"] = 255;
                    break;
                case TypeCode.Char:
                    // ? quandry - represent char as a string of length 1 or as an uint16?
                    // type: "string" maxLength: 1
                    typeObj["type"] = "string";
                    typeObj["minLength"] = 1;
                    typeObj["maxLength"] = 1;
                    break;
                case TypeCode.DateTime:
                    // type: "string" format: "wcf-date" <-- negotiable depending on service serialization 
                    typeObj["type"] = "string";
                    typeObj["format"] = "wcf-date";
                    break;
                case TypeCode.Decimal:
                    // type: "number" format: "decimal" minValue:-79228162514264337593543950335,maxValue:79228162514264337593543950335, divisibleBy:0.01 <-- defines precision of 2

                    typeObj["type"] = "number";
                    typeObj["format"] = "decimal";
                    typeObj["minValue"] = -79228162514264337593543950335m;
                    typeObj["maxValue"] = 79228162514264337593543950335m;
                    break;

                case TypeCode.Double:
                    // type: "number", minValue: -1.79769313486231e308 (one more than .net  else inifinty),maxValue: 1.79769313486231e308 (one less than .net else inifinty)
                    typeObj["type"] = "number";
                    typeObj["format"] = "decimal";
                    typeObj["minValue"] = double.MinValue + 1; // minvalue is -infinity in IE js vm
                    typeObj["maxValue"] = double.MaxValue - 1;// maxValue is infinity in IE js vm
                    break;
                    break;
                case TypeCode.Int16:
                    typeObj["type"] = "integer";
                    typeObj["format"] = "short";
                    typeObj["minValue"] = -32768;
                    typeObj["maxValue"] = 32767;
                    break;
                case TypeCode.Int32:
                    typeObj["type"] = "integer";
                    typeObj["minValue"] = -2147483648;
                    typeObj["maxValue"] = 2147483647;
                    break;
                case TypeCode.Int64:
                    typeObj["type"] = "integer";
                    typeObj["format"] = "long";
                    typeObj["minValue"] = -9223372036854775808;
                    typeObj["maxValue"] = 9223372036854775807;

                    break;
                case TypeCode.SByte:
                    typeObj["type"] = "integer";
                    typeObj["format"] = "sbyte";
                    typeObj["minValue"] = -128;
                    typeObj["maxValue"] = 127;
                    break;

                case TypeCode.Single:
                    typeObj["type"] = "number";
                    typeObj["format"] = "single";
                    typeObj["minValue"] = -3.402823e38;
                    typeObj["maxValue"] = 3.402823e38;
                    break;

                case TypeCode.String:
                    typeObj["type"] = "string";
                    break;
                case TypeCode.UInt16:
                    typeObj["type"] = "integer";
                    typeObj["format"] = "ushort";
                    typeObj["minValue"] = 0;
                    typeObj["maxValue"] = 65535;
                    break;
                case TypeCode.UInt32:
                    typeObj["type"] = "integer";
                    typeObj["format"] = "uint";
                    typeObj["minValue"] = 0;
                    typeObj["maxValue"] = 4294967295;
                    break;
                case TypeCode.UInt64:
                    typeObj["type"] = "integer";
                    typeObj["format"] = "ulong";
                    typeObj["minValue"] = 0;
                    typeObj["maxValue"] = 18446744073709551615;
                    break;

                case TypeCode.Object:
                    JObject obj = new JObject();
                    obj["$ref"] = type.Name;
                    typeObj["type"] = obj;
                    break;
                case TypeCode.DBNull:
                case TypeCode.Empty:
                    throw new NotSupportedException("unsupported type " + typecode);

                default:
                    throw new DefectException("typecode from outerspace");
            }

            return typeObj;
        }

        public static void ApplyPropertyAttribute(JObject propBase, string attributeValue, string parentName, string name)
        {
            // quick munge to clean up xml docs
            name = MungeAttributeName(name);

            switch (name)
            {
                // handle type agnostic attributes first

                // boolean
                case "optional":
                case "required":
                case "additionalProperties":
                case "uniqueItems":
                case "minimumCanEqual":
                case "maximumCanEqual":
                    propBase[name] = Convert.ToBoolean(attributeValue);
                    break;
                // string
                case "pattern":
                case "title":
                case "description":
                case "format":
                case "contentEncoding":
                case "extends":
                    propBase[name] = attributeValue;
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
                        propBase[name] = Convert.ToDecimal(attributeValue);
                    }
                    else if (propTypeString == "integer")
                    {
                        propBase[name] = Convert.ToInt64(attributeValue);
                    }
                    else
                    {
                        throw new InvalidOperationException("invalid property type for attribute min/max\n" + string.Format("Type:{0}, Member:{1}, Attribute:{2}", parentName, name, name));
                    }
                    break;
                case "minItems":
                case "maxItems":
                case "maxLength":
                case "minLength":
                    propBase[name] = Convert.ToInt64(attributeValue);
                    break;
                case "default":
                    // TODO:
                    // ApplyTypedValue(propBase, attribute, false);
                    break;
                case "enum":
                    throw new NotImplementedException("enum is constructed by use of underlying type attributeName\n" + string.Format("Type:{0}, Member:{1}, Attribute:{2}", parentName, name, name));


                case "requires":
                case "divisibleBy":
                case "disallow":
                    throw new NotImplementedException(name + " is not supported\n" + string.Format("Type:{0}, Member:{1}, Attribute:{2}", parentName, name, name));
                default:
                    // ignore all of the rest
                    break;
            }
        }

        /// <summary>
        /// This will require a bit of special casing. 
        /// demoValue needs to be applied to the body of the property, including
        /// collection types. 
        /// 
        /// Will do string comparison on the type to allow for future handling
        /// of nullable union types
        /// </summary>
        public static void ApplyTypedValue(JObject propBase, XAttribute attribute, bool treatAsLiteral)
        {
            try
            {

                if (attribute == null)
                {
                    throw new ArgumentException("attribute is null");
                }

                JToken propTypeAttribute = propBase["type"];
                var propType = propTypeAttribute != null ? propTypeAttribute.Value<string>() : "string";
                var propertyName = attribute.Name.ToString();
                var propertyValue = attribute.Value;

                if (treatAsLiteral)
                {
                    var literal = (JToken)JsonConvert.DeserializeObject(propertyValue);
                    propBase.Add(propertyName, literal);
                }
                else
                {



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

            }
            catch (Exception e)
            {
                string errorMessage = string.Format("An exception occured when applying attribute\n\n{1}\n\nto\n\n{0}", propBase, attribute);
                throw new Exception(errorMessage, e);

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
                propBase["description"] = summary.Trim();
            }
        }

        /// <summary>
        /// CAVEATS:
        ///   Does not handle Arrays/Lists of Arrays/Lists
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="type"></param>
        private static void RenderTypeMeta(JObject jobj, Type type)
        {
            var isArray = false;

            if (type.IsArray)
            {
                isArray = true;
                type = type.GetElementType();
            }
            else if (type.IsListType())
            {
                isArray = true;
                type = type.GetGenericArguments()[0];
            }



            var isNullable = type.IsNullableType();

            if (isNullable)
            {
                type = type.GetGenericArguments()[0];
            }



            // at this point type variable should contain the underlying type

            
            JObject typeObj;

            typeObj = GetSchemaType(type);
            
            if (isNullable)
            {
                if (isArray)
                {
                    jobj["type"] = "array";
                    jobj["items"] = new JArray("null", typeObj["type"]);
                }
                else
                {
                    jobj["type"] = typeObj["type"];
                }
            }
            else
            {
                if (isArray)
                {
                    jobj["type"] = "array";
                    jobj["items"] = new JArray(typeObj["type"]);
                }
                else
                {
                    jobj["type"] = typeObj["type"];
                }
            }

            // type is the complicated aspect
            // get it out of the way so we can apply
            // the other constraints that apply to 
            // the underlying type 
            typeObj.Remove("type");

            foreach (var item in typeObj)
            {
                jobj[item.Key] = item.Value;
            }
            

        }

    }
}
