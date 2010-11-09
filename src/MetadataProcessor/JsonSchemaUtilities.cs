using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using MetadataProcessor.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TradingApi.Configuration;

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
                jsPropertyValue.Add("$ref", "#.properties." + targetType.Name);
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

        public static void ApplyDescription(JObject propBase, XElement propElement)
        {
            var summaryElement = propElement.Descendants("summary").FirstOrDefault();
            if (summaryElement != null)
            {
                propBase.Add("description", summaryElement.Value.Trim());
            }
        }

        public static void ApplyPropertyAttribute(JObject propBase, XAttribute attribute)
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
                        throw new InvalidOperationException("invalid property type for attribute min/max");
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
                    throw new NotImplementedException("enum is constructed by use of underlying type attributeName");


                case "requires":
                case "divisibleBy":
                case "disallow":
                    throw new NotImplementedException(attributeName + " is not supported");
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
                            jsob.Add(new JProperty("extends", new JObject(new JProperty("$ref", "#.properties." + type.BaseType.Name))));
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
                                    BuildPropertySchema(propElement, metaElement, includeDemoValue, schemaProperties, property.Name, property.PropertyType);
                                }

                            }


                        }
                        
                    }

                    

                    return jsob;
                }
            }
            return null;
        }


        public static JObject BuildPropertySchema(XElement docElement, XElement metaElement, bool includeDemoValue, JObject metaContainer, string propertyName, Type propertyType)
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
                ApplyPropertyAttribute(attributeTarget, attribute);
            }

            return propBase;
        }


        public static JObject BuildParameterSchema(XElement docElement, XElement metaElement, bool includeDemoValue, JArray metaContainer, string propertyName, Type propertyType)
        {
            // no jschema, no process

            var underlyingType = metaElement.Attributes("underlyingType").FirstOrDefault();

            if (underlyingType != null)
            {
                propertyType = Type.GetType(underlyingType.Value, true, false);
            }


            JObject propBase = BuildPropertyBase(propertyType);
            propBase.Add("name", propertyName);

            metaContainer.Add(propBase);

            if (!string.IsNullOrWhiteSpace(metaElement.Value))
            {
                propBase.Add("description", metaElement.Value.Trim());
            }


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
                ApplyPropertyAttribute(attributeTarget, attribute);
            }
            return propBase;
        }

        public static JObject BuildSMDBase(string smdUrl, string smdTargetUrl, string smdSchemaUrl, string smdDescription, string apiVersion, string smdVersion)
        {
            JObject smd = new JObject
		            {
		                {"SMDVersion",smdVersion},
		                {"version",apiVersion},
		                {"id",smdUrl},
		                {"target",smdTargetUrl},
		                {"schema",smdSchemaUrl},
		                {"description",smdDescription},
		                {"additionalParameters",true},
		                {"useUriTemplates",true},
		                {"contentType","application/json"},
		                {"responseContentType","application/json"},
		                {"services", new JObject()}
		            };
            return smd;
        }

        public static void BuildServiceMapping(UrlMapElement route, List<Type> mappedTypes, JObject smdBase, List<AssemblyReferenceElement> dtoAssemblies, bool includeDemoValue)
        {
            var type = Type.GetType(route.Type);
            var doc = GetXmlDocs(type);

            if (mappedTypes.Contains(type))
                return;
            mappedTypes.Add(type);



            var typeElement = GetMemberNode(doc, "T:" + type.FullName);
            var typeSmdElement = typeElement.Descendants("smd").FirstOrDefault();
            if (typeSmdElement == null)
                return;




            var targetBase = route.Endpoint;



            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {

                //NOTE: Under certain circumstances, WCF supports overloading - this SMD generation code does not.
                // this element fetch would need to be extended to specify type and order of parameters to support overloading.
                var methodElement = GetMemberNodes(doc, "M:" + type.FullName + "." + method.Name).FirstOrDefault();
                var methodSmdElement = methodElement.Descendants("smd").FirstOrDefault();
                if (methodSmdElement == null)
                    continue;

                JObject service = null;
                var opContract = ReflectionUtils.GetAttribute<OperationContractAttribute>(method);
                if (opContract != null)
                {
                    // get smd xml, if present

                    var webGet = ReflectionUtils.GetAttribute<WebGetAttribute>(method);
                    string methodName = method.Name;
                    XAttribute methodSmdElementMethodAttribute = methodSmdElement.Attributes("method").FirstOrDefault();
                    if (methodSmdElementMethodAttribute != null)
                    {
                        methodName = methodSmdElementMethodAttribute.Value;
                    }
                    string methodTarget = null;
                    string methodTransport = null;
                    if (webGet != null)
                    {
                        service = new JObject();

                        methodTarget = targetBase + webGet.UriTemplate;
                        methodTransport = "GET";
                    }
                    else
                    {
                        var webInvoke = ReflectionUtils.GetAttribute<WebInvokeAttribute>(method);
                        if (webInvoke != null)
                        {
                            // DAVID: NOTE: ignore problematic methods - using DELETE is simply problematic in many
                            // aspects of a rest client, especially javascript. With the introduction of the extra logout
                            // method, we can eliminate the only DELETE and move forward using GET/POST only
                            if (webInvoke.Method == "POST")
                            {
                                service = new JObject();
                                methodTarget = targetBase + webInvoke.UriTemplate;
                                methodTransport = "POST";
                            }
                        }
                    }

                    if (service != null)
                    {
                        ApplyDescription(service, methodElement);
                        service.Add("target", methodTarget.TrimEnd('/'));
                        service.Add("transport", methodTransport);
                        ((JObject)smdBase["services"]).Add(methodName, service);

                        // this is not accurate/valid SMD for GET but dojox.io.services is not, yet, a very good 
                        // implementation of the SMD spec, which is funny as they were both written by the same person.
                        service.Add("envelope", "JSON");
                        service.Add("returns", new JObject(new JProperty("$ref", "#.properties." + method.ReturnType.Name))); //NOTE: scalar return types are not indicated by API and are not supported by this code
                        var parameters = new JArray();
                        service.Add("parameters", parameters);


                        // flesh out the parameters

                        foreach (var parameter in method.GetParameters())
                        {

                            // is this a DTO?
                            Type parameterType = parameter.ParameterType;
                            var parameterAssemblyFullName = parameterType.Assembly.FullName;
                            bool isDTO = false;

                            foreach (AssemblyReferenceElement assemblyName in dtoAssemblies)
                            {
                                if (assemblyName.Assembly.StartsWith(parameterAssemblyFullName))
                                {
                                    isDTO = true;
                                    break;
                                }
                            }

                            if (isDTO)
                            {
                                // break dto down into properties and include each as a parameter
                                // use jsonschema to build parameters
                                XDocument dtoDoc = JsonSchemaUtilities.GetXmlDocs(parameter.ParameterType);
                                foreach (PropertyInfo parameterProperty in parameter.ParameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                                {

                                    XElement propElement = JsonSchemaUtilities.GetMemberNode(dtoDoc, "P:" + parameter.ParameterType.FullName + "." + parameterProperty.Name);

                                    if (propElement != null)
                                    {
                                        XElement metaElement = propElement.Descendants("jschema").FirstOrDefault();
                                        if (metaElement != null)
                                        {
                                            JsonSchemaUtilities.BuildParameterSchema(propElement, metaElement, includeDemoValue, parameters, parameterProperty.Name, parameterProperty.PropertyType);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var metaElement = methodElement.Descendants("param").Where(p => p.Attribute("name").Value == parameter.Name).First();
                                JsonSchemaUtilities.BuildParameterSchema(methodElement, metaElement, includeDemoValue, parameters, parameter.Name, parameter.ParameterType);

                            }
                        }

                    }

                }

            }
        }
    }
}