using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

using MetadataProcessor;
using MetadataProcessor.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TradingApi.Configuration;

namespace JsonSchemaGeneration.WcfSMD
{
    public class Emitter
    {

        public string EmitSmdJson(IEnumerable<UrlMapElement> routes, bool includeDemoValue, string[] dtoAssemblyNames, string patchJson)
        {
            JObject patch = (JObject)JsonConvert.DeserializeObject(patchJson);
            JObject smd = new JObject
                              {
                                  {"SMDVersion","2.6"},
                                  {"version","1"},
                                  {"description","CIAPI SMD"},
                                  {"services", new JObject()}
                              };

            var rpc = new JObject();
            smd["services"]["rpc"] = rpc;
            rpc["target"] = "";
            JObject rpcServices = new JObject();
            rpc["services"] = rpcServices;

            var seenTypes = new List<Type>(); // just to keep track of types so we don't map twice

            foreach (UrlMapElement route in routes)
            {


                try
                {

                    BuildServiceMapping(route, seenTypes, rpcServices, includeDemoValue, dtoAssemblyNames, patch);
                }
                catch (Exception exc)
                {
                    string errorMessage = string.Format("Error Building the Service Mapping for Service . {0}: {1}", Type.GetType(route.Type), exc);
                    throw new Exception(errorMessage);
                }
            }


            string result = smd.ToString();
            return result;
        }

        private void BuildServiceMapping(UrlMapElement route, List<Type> seenTypes, JObject smdBase, bool includeDemoValue, string[] dtoAssemblyNames, JObject patch)
        {


            Type type = Assembly.Load(route.Type.Substring(route.Type.IndexOf(",") + 1).Trim()).GetType(route.Type.Substring(0, route.Type.IndexOf(",")));
            if (seenTypes.Contains(type))
            {
                return;
            }

            seenTypes.Add(type);



            var typeElement = type.GetXmlDocTypeNodeWithSMD();

            if (typeElement == null)
            {
                return;
            }




            var methodTarget = route.Endpoint.Trim('/');

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {

                
                if (patch["smd"][method.Name] != null)
                {
                    if (patch["smd"][method.Name].Type == JTokenType.String && patch["smd"][method.Name].Value<string>() == "exclude")
                    {
                        continue;
                    }
                }

                JObject methodPatch = (JObject)patch["smd"][method.Name];

                var methodElement = type.GetXmlDocMemberNodeWithSMD(type.FullName + "." + method.Name);
                if (methodElement == null)
                {
                    continue;
                }
                var methodSmdElement = methodElement.XPathSelectElement("smd");

                if (methodSmdElement == null)
                {
                    throw new DefectException("should not have gotten a method element without smd");
                }

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
                        if(methodPatch!=null)
                        {
                            if(methodPatch["uriTemplate"]!=null )
                            {
                                methodUriTemplate = methodPatch["uriTemplate"].Value<string>();

                            }
                        }
                        if (!string.IsNullOrWhiteSpace(methodUriTemplate))
                        {
                            service.Add("uriTemplate", methodUriTemplate);
                        }


                        service.Add("contentType", "application/json");// TODO: declare this in meta or get from WebGet/WebInvoke
                        service.Add("responseContentType", "application/json");// TODO: declare this in meta or get from WebGet/WebInvoke
                        service.Add("transport", methodTransport);

                        smdBase.Add(methodName, service);

                        // this is not accurate/valid SMD for GET but dojox.io.services is not, yet, a very good 
                        // implementation of the SMD spec, which is funny as they were both written by the same person.
                        service.Add("envelope", methodEnvelope);
                        JObject returnType = new JObject(new JProperty("$ref",  method.ReturnType.Name));
                        // TODO: go ahead and add description to reference. we are the only people in the world that are
                        // excercising json-schema and smd to this extent. let them follow us.
                        service.Add("returns", returnType); //NOTE: scalar return types are not indicated by API and are not supported by this code



                        SetStringAttribute(methodSmdElement, service, "group");
                        SetIntAttribute(methodSmdElement, service, "cacheDuration");
                        SetStringAttribute(methodSmdElement, service, "throttleScope");

                        AddParameters(type, method, methodElement, service, dtoAssemblyNames, includeDemoValue,methodPatch);
                    }

                }

            }



        }

        

        private static void AddParameters(Type type, MethodInfo method, XElement methodElement, JObject service, IEnumerable<string> dtoAssemblyNames, bool includeDemoValue,JObject methodPatch)
        {
            var parameters = new JArray();
            service.Add("parameters", parameters);

            foreach (var parameter in method.GetParameters())
            {
                var metaElement =
                    methodElement.Descendants("param").Where(p => p.Attribute("name").Value == parameter.Name).
                        FirstOrDefault();
                if (metaElement == null)
                {
                    // check patch
                    if (methodPatch["parameters"][parameter.Name] != null)
                    {
                        parameters.Add(methodPatch["parameters"][parameter.Name]);    
                    }
                    else
                    {
                        string message = string.Format("param element not found for {0}.{1} - {2}", type.Name, method.Name, parameter.Name);
                        throw new Exception(message);    
                    }
                    
                    
                }
                else
                {
                    BuildParameterSchema(methodElement, metaElement, includeDemoValue, parameters, parameter.Name,
                                         parameter.ParameterType, type.FullName);
                }

            }
        }

        public static JObject BuildParameterSchema(XElement docElement, XElement metaElement, bool includeDemoValue, JArray metaContainer, string propertyName, Type propertyType, string parentName)
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
                if (demoValueAttribute == null)
                {
                    //if (!isComplexType)
                    //{
                    //    throw new Exception(
                    //        string.Format("includeDemoValue is true but {0}.{1} has no demoValue attribute", parentName,
                    //                      propertyName));                        
                    //}

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
