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
        public void CheckSchema(string schema, params Assembly[] assemblies)
        {
            
            var schemaObj = (JObject)JsonConvert.DeserializeObject(schema);
            var exposedTypes = new List<Type>();

            foreach (Assembly assembly in assemblies)
            {
                var docXml = XmlDocExtensions.GetXmlDocs(assembly.GetTypes()[0]);

                foreach (Type type in assembly.GetTypes())
                {
                    CheckType(docXml, type, schemaObj);
                }
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
            if (typeNode.Descendants("jschema").FirstOrDefault() != null)
            {
                EnsureTypeInSchema(schemaObj, type.Name);

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
}
