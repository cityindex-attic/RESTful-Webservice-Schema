using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml.Linq;
using MetadataProcessor.Utilities;
using Newtonsoft.Json.Linq;
using TradingApi.Configuration;

namespace MetadataProcessor
{
    public static class SmdGenerator
    {
        public static void BuildServiceMapping(UrlMapElement route, List<Type> mappedTypes, JObject smdBase, List<AssemblyReferenceElement> dtoAssemblies, bool includeDemoValue)
        {

            var type = Type.GetType(route.Type);

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
                        methodEnvelope = "URL";
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

                        service.Add("transport", methodTransport);

                        ((JObject)smdBase["services"]).Add(methodName, service);

                        // this is not accurate/valid SMD for GET but dojox.io.services is not, yet, a very good 
                        // implementation of the SMD spec, which is funny as they were both written by the same person.
                        service.Add("envelope", methodEnvelope);
                        JObject returnType = new JObject(new JProperty("$ref", "#." + method.ReturnType.Name));
                        // TODO: go ahead and add description to reference. we are the only people in the world that are
                        // excercising json-schema and smd to this extent. let them follow us.
                        service.Add("returns", returnType); //NOTE: scalar return types are not indicated by API and are not supported by this code


                        string methodGroup = null;
                        XAttribute methodSmdElementGroupAttribute = methodSmdElement.Attributes("group").FirstOrDefault();
                        if (methodSmdElementGroupAttribute != null)
                        {
                            methodGroup = methodSmdElementGroupAttribute.Value;
                            service.Add("group", methodGroup);
                        }

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

                }

            }


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
                                  {"contentType","application/json"},
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
            

            if (!string.IsNullOrWhiteSpace(metaElement.Value))
            {
                propBase.Add("description", metaElement.Value.Trim());
            }


            if (includeDemoValue)
            {
                var demoValueAttribute = metaElement.Attributes("demoValue").FirstOrDefault();

                if (demoValueAttribute != null)
                {
                    JsonSchemaUtilities.ApplyTypedValue(propBase, demoValueAttribute);
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

            if (propBase["optional"] == null)
            {
                propBase.Add("optional", false);
            }
            return propBase;
        }
    }
}