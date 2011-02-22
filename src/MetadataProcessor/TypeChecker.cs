using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MetadataProcessor
{

    /// <summary>
    /// Performs sanity checks to ensure all exposed DTO types are properly referenced/described across
    /// assemblies/smd/schema.
    /// Also ensure that types referenced by property types are exposed via JSchema
    /// Also ensure that base types are exposed via JSchema
    /// </summary>
    /// <exception cref="Exception">If a type is missing from SMD or Schema</exception>
    public class TypeChecker
    {
        /// <summary>
        /// central assembler of schema verification methods
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="smd"></param>
        /// <param name="verifyDemoValues"></param>
        public void VerifySmd(string schema, string smd, bool verifyDemoValues)
        {
            VerifySmdReturnTypes(schema, smd);


        }

        /// <summary>
        /// Verifies only return types. 
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="smd"></param>
        public void VerifySmdReturnTypes(string schema, string smd)
        {
            var schemaObj = (JObject)JsonConvert.DeserializeObject(schema);
            var smdObj = (JObject)JsonConvert.DeserializeObject(smd);
            foreach (JProperty token in smdObj["services"])
            {
                JToken returnsToken = token.Value["returns"];
                if (returnsToken["$ref"] != null)
                {
                    string returnType = returnsToken["$ref"].Value<string>().Substring(2);
                    var type = schemaObj[returnType];
                    if (type == null)
                    {
                        throw new Exception(string.Format("Method {0} return type {1} is not represented in schema", token.Name, returnType));
                    }
                }

            }
        }

        public void CheckSchema(string schema, params Assembly[] assemblies)
        {

            var schemaObj = (JObject)JsonConvert.DeserializeObject(schema);
            var exposedTypes = new List<Type>();
            MetaVerificationException aggregatedException = null;
            foreach (Assembly assembly in assemblies)
            {
                var docXml = XmlDocExtensions.GetXmlDocs(assembly.GetTypes()[0]);

                foreach (Type type in assembly.GetTypes())
                {
                    try
                    {
                        CheckType(docXml, type, schemaObj);

                    }
                    catch (Exception ex)
                    {

                        if (aggregatedException == null)
                        {
                            aggregatedException = new MetaVerificationException();
                        }
                        aggregatedException.Messages.Add(ex.Message);
                    }

                    
                }
            }
            if (aggregatedException != null)
            {
                throw aggregatedException;
            }
        }




        private static void CheckType(XDocument docXml, Type type, JObject schemaObj)
        {
            var typeNode = JsonSchemaUtilities.GetMemberNode(docXml, "T:" + type.FullName);
            if (typeNode == null)
            {
                // if it is not in the xml then it would not be in the schema or smd under any 
                // circumstances and we can/should do nothing but move on to the next type
                return;

            }
            
     

                // if xml specifies jschema then we need to check the deserialized json schema 
                XElement jschemaNode = typeNode.Descendants("jschema").FirstOrDefault();
                if (jschemaNode != null && ShouldTypeShouldBeIncluded(jschemaNode))
                {
                    EnsureTypeInSchema(schemaObj, type.Name);

                    // if derived, recursively ensure base type in schema.
                    if (!type.BaseType.FullName.StartsWith("System"))
                    {
                        var baseDocXml = XmlDocExtensions.GetXmlDocs(type.BaseType);
                        CheckType(baseDocXml, type.BaseType, schemaObj);
                    }

                    // check each property
                    foreach (var propInfo in type.GetProperties())
                    {
                        if (propInfo.PropertyType.IsClass || propInfo.PropertyType.IsEnum)
                        {

                            // if is nullable type get underlying type
                            type = JsonSchemaUtilities.GetNullableTypeIfAny(type);

                            // if it is a collection type, get element type otherwise prop type
                            // NOTE: arrays of arrays (or lists of lists etc) are not supported by this check
                            var propType = JsonSchemaUtilities.GetCollectionType(propInfo.PropertyType) ?? propInfo.PropertyType;

                            // check again, if is nullable type get underlying type
                            propType = JsonSchemaUtilities.GetNullableTypeIfAny(propType);

                            // avoid intrinsic types that are classes
                            switch (propType.Name)
                            {
                                case "String":
                                case "DateTime":
                                case "DateTimeOffset":
                                    // others?
                                    continue;
                                default:
                                    break;
                            }

                            EnsureTypeInSchema(schemaObj, propType.Name);
                        }


                        // could recurse here but the way i see it is if the property type is a class
                        // and it is present in the schema then it is also exposed on the assembly and will
                        // get checked.
                    }
                }
      
        }

        private static bool ShouldTypeShouldBeIncluded(XElement jschemaNode)
        {
            return ((jschemaNode.Attributes("output").FirstOrDefault() != null && jschemaNode.Attributes("output").First().Value == "true") || jschemaNode.Attributes("output").FirstOrDefault() == null);
        }

        /// <summary>
        /// TODO: create aggregate exception type so that we can report all problems
        /// at once instead of failing on the first missing type
        /// </summary>
        /// <param name="schemaObj"></param>
        /// <param name="name"></param>
        private static void EnsureTypeInSchema(JObject schemaObj, string name)
        {
            var obj = schemaObj[name];
            if (obj == null)
            {
                throw new Exception(string.Format("type {0} is not represented in schema", name));
            }
            var value = obj.Value<JObject>();

            if (value == null)
            {
                throw new Exception(string.Format("type {0} is not represented in schema", name));
            }

        }
    }

    public class MetaVerificationException : Exception
    {
        public override string Message
        {
            get
            {
                return string.Join(Environment.NewLine, Messages);
            }
        }
        private List<string> _messages = new List<string>();
        public List<string> Messages
        {
            get
            {
                return _messages;
            }
            set
            {
                _messages = value;
            }
        }
    }
}
