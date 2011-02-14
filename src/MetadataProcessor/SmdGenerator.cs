using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using System.Xml.Linq;
using MetadataProcessor.Utilities;
using Newtonsoft.Json.Linq;

namespace MetadataProcessor
{
    public static class SmdGenerator
    {
        public static JObject BuildSMD(HttpContext context, bool includeDemoValue, string smdUrl, string smdTargetUrl, string smdSchemaUrl, string apiVersion, string smdDescription, string smdVersion, List<string> dtoAssemblyNames, IEnumerable<UrlMapInfo> routes)
        {
            var smdBase = SmdGenerator.BuildSMDBase(smdUrl, smdTargetUrl, smdSchemaUrl, smdDescription, apiVersion, smdVersion, includeDemoValue);
            var mappedTypes = new List<Type>(); // just to keep track of types so we don't map twice

            foreach (UrlMapInfo route in routes)
            {
                var serviceRoute = new ServiceRoute
                {
                    Name = route.Name,
                    Endpoint = route.Endpoint,
                    ServiceType = Type.GetType(route.Type)
                };

                try
                {

                    SmdGenerator.BuildServiceMapping(serviceRoute, mappedTypes, dtoAssemblyNames, smdBase, includeDemoValue);
                }
                catch (Exception exc)
                {
                    string errorMessage = string.Format("Error Building the Service Mapping for Service . {0}: {1}", Type.GetType(route.Type), exc);
                    if (context != null)
                    {
                        context.Response.Write(errorMessage);
                    }

                    throw new Exception(errorMessage);
                }
            }







            return smdBase;
        }

        private static void SetStringAttribute(XElement methodSmdElement, JObject service, string attributeName)
        {
            XAttribute throttleScopeAttribute = methodSmdElement.Attributes(attributeName).FirstOrDefault();
            if (throttleScopeAttribute != null)
            {
                service.Add(attributeName, throttleScopeAttribute.Value);
            }
        }

        private static void SetIntAttribute(XElement methodSmdElement, JObject service, string attributeName)
        {
            XAttribute throttleScopeAttribute = methodSmdElement.Attributes(attributeName).FirstOrDefault();
            if (throttleScopeAttribute != null)
            {
                service.Add(attributeName, Convert.ToInt64(throttleScopeAttribute.Value));
            }
        }
        public static void BuildServiceMapping(ServiceRoute route, List<Type> mappedTypes, List<string> dtoAssemblyNames, JObject smdBase, bool includeDemoValue)
        {

            var type = route.ServiceType;

            var doc = JsonSchemaUtilities.GetXmlDocs(type);

            if (mappedTypes.Contains(type))
            {
                return;
            }

            mappedTypes.Add(type);



            var typeElement = JsonSchemaUtilities.GetMemberNode(doc, "T:" + type.FullName);
            var typeSmdElement = typeElement.Descendants("smd").FirstOrDefault();
            if (typeSmdElement == null)
                return;




            var methodTarget = route.Endpoint.Trim('/');



            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {

                //NOTE: Under certain circumstances, WCF supports overloading - this SMD generation code does not.
                // this element fetch would need to be extended to specify type and order of parameters to support overloading.
                var methodElement = JsonSchemaUtilities.GetMemberNodes(doc, "M:" + type.FullName + "." + method.Name).FirstOrDefault();
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


                    string methodTransport = null;
                    string methodEnvelope = null;
                    string methodUriTemplate = null;
                    if (webGet != null)
                    {
                        service = new JObject();
                        methodUriTemplate = FixUriTemplate(webGet.UriTemplate);
                        methodTransport = "GET";
                        // TODO: FIXME: for some reason this is returning json string instead of json object when flxhr is used
                        // is an odd interaction between flxhr and dojox.rpc.Service
                        // HACK: use JSON envelope for now - results in not entirely correct smd but dojox behaves as it should
                        methodEnvelope = "URL";
                        //methodEnvelope = "JSON"; 
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
                                methodUriTemplate = FixUriTemplate(webInvoke.UriTemplate);

                                methodTransport = "POST";
                                methodEnvelope = "JSON";
                            }
                        }
                    }

                    if (service != null)
                    {
                        JsonSchemaUtilities.ApplyDescription(service, methodElement);
                        service.Add("target", methodTarget.TrimEnd('/'));
                        if (!string.IsNullOrWhiteSpace(methodUriTemplate))
                        {
                            service.Add("uriTemplate", methodUriTemplate);
                        }


                        service.Add("contentType", "application/json");// TODO: declare this in meta or get from WebGet/WebInvoke
                        service.Add("responseContentType", "application/json");// TODO: declare this in meta or get from WebGet/WebInvoke
                        service.Add("transport", methodTransport);

                        ((JObject)smdBase["services"]).Add(methodName, service);

                        // this is not accurate/valid SMD for GET but dojox.io.services is not, yet, a very good 
                        // implementation of the SMD spec, which is funny as they were both written by the same person.
                        service.Add("envelope", methodEnvelope);
                        JObject returnType = new JObject(new JProperty("$ref", "#." + method.ReturnType.Name));
                        // TODO: go ahead and add description to reference. we are the only people in the world that are
                        // excercising json-schema and smd to this extent. let them follow us.
                        service.Add("returns", returnType); //NOTE: scalar return types are not indicated by API and are not supported by this code



                        SetStringAttribute(methodSmdElement, service, "group");
                        SetIntAttribute(methodSmdElement, service, "cacheDuration");
                        SetStringAttribute(methodSmdElement, service, "throttleScope");

                        AddParameters(type, method, methodElement, service, dtoAssemblyNames, includeDemoValue);
                    }

                }

            }
        }

        /// <summary>
        /// flesh out the parameters
        /// </summary>
        private static void AddParameters(Type type, MethodInfo method, XElement methodElement, JObject service, List<string> dtoAssemblyNames, bool includeDemoValue)
        {
            var parameters = new JArray();
            service.Add("parameters", parameters);

            foreach (var parameter in method.GetParameters())
            {
                if (IsParameterADto(parameter, dtoAssemblyNames))
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
                                BuildParameterSchema(propElement, metaElement, includeDemoValue, parameters, parameterProperty.Name, parameterProperty.PropertyType, type.FullName);
                            }
                        }
                    }
                }
                else
                {
                    var metaElement = methodElement.Descendants("param").Where(p => p.Attribute("name").Value == parameter.Name).First();
                    BuildParameterSchema(methodElement, metaElement, includeDemoValue, parameters, parameter.Name, parameter.ParameterType, type.FullName);
                }
            }
        }

        private static bool IsParameterADto(ParameterInfo parameter, List<string> dtoAssemblyNames)
        {
            Type parameterType = parameter.ParameterType;
            var parameterAssemblyFullName = parameterType.Assembly.FullName;
            bool isDTO = false;

            foreach (var assemblyName in dtoAssemblyNames)
            {
                if (parameterAssemblyFullName.StartsWith(assemblyName))
                {
                    isDTO = true;
                    break;
                }
            }
            return isDTO;
        }

        public static JObject BuildSMDBase(string smdUrl, string smdTargetUrl, string smdSchemaUrl, string smdDescription, string apiVersion, string smdVersion, bool includeDemoValue)
        {
            JObject smd = new JObject
                              {
                                  {"SMDVersion",smdVersion},
                                  {"version",apiVersion},
                                  {"id",smdUrl},
                                  {"target",smdTargetUrl},
                                  {"schema",smdSchemaUrl+ (includeDemoValue ? "?includeDemoValue=true" : "")},
                                  {"description",smdDescription},
                                  {"additionalParameters",true},
                                  {"services", new JObject()}
                              };
            return smd;
        }

        private static string FixUriTemplate(string methodUriTemplate)
        {
            if (!string.IsNullOrWhiteSpace(methodUriTemplate))
            {
                methodUriTemplate = methodUriTemplate.Replace("/?", "?");

                //if (methodUriTemplate == "/")
                //{
                //    methodUriTemplate = null;
                //}
            }
            return methodUriTemplate;
        }

        
        public static JObject BuildParameterSchema(XElement docElement, XElement metaElement, bool includeDemoValue, JArray metaContainer, string propertyName, Type propertyType,string parentName)
        {
            // no jschema, no process

            var underlyingType = metaElement.Attributes("underlyingType").FirstOrDefault();

            if (underlyingType != null)
            {
                propertyType = Type.GetType(underlyingType.Value, true, false);
            }


            JObject propBase = JsonSchemaUtilities.BuildPropertyBase(propertyType);
            propBase.Add("name", propertyName);

            metaContainer.Add(propBase);
            
            AddPropertyDescription(propBase, metaElement, docElement);

            if (includeDemoValue)
            {
                var demoValueAttribute = metaElement.Attributes("demoValue").FirstOrDefault();

                // if type is class and not System.* then the demoValue indicates embedded JSON. 
                // typically this is the case when the shape of the type is problematic due to recursion
                // or circular references

                bool isComplexType = JsonSchemaUtilities.IsComplexType(propertyType);


                // we do not force demo values on complex types. if one is not present, the js will try to compose one.
                if (demoValueAttribute == null )
                {
                    if (!isComplexType)
                    {
                        throw new Exception(
                            string.Format("includeDemoValue is true but {0}.{1} has no demoValue attribute", parentName,
                                          propertyName));                        
                    }

                }

                else
                {
                    JsonSchemaUtilities.ApplyTypedValue(propBase, demoValueAttribute, isComplexType);
                }
            }


            JObject attributeTarget = propBase;
            if (propBase["type"] != null && propBase["type"].Value<string>() == "array")
            {
                attributeTarget = propBase["items"].Value<JObject>();
            }

            foreach (var attribute in metaElement.Attributes())
            {
                JsonSchemaUtilities.ApplyPropertyAttribute(attributeTarget, attribute, parentName, propertyName);
            }

            //if (propBase["required"] == null)
            //{
            //    propBase.Add("optional", false);
            //}
            return propBase;
        }

        internal static void AddPropertyDescription(JObject propBase, XElement metaElement, XElement docElement)
        {
            if (TryAddPropertyDescription(metaElement.Value, propBase)) return;

            var summary = docElement.Elements()
                    .Where(e => e.Name.LocalName.ToLower() == "summary")
                    .Select(e => e.Value).FirstOrDefault();
            TryAddPropertyDescription(summary, propBase);
        }

        private static bool TryAddPropertyDescription(string description, JObject propBase)
        {
            if (string.IsNullOrWhiteSpace(description)) return false;

            propBase.Add("description", description.Trim());
            return true;
        }


    }
}