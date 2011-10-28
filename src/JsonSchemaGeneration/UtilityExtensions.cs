using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JsonSchemaGeneration.JsonSchemaDTO;

namespace JsonSchemaGeneration
{
    public static class UtilityExtensions
    {
        public static IEnumerable<Type> GetSchemaTypes(IEnumerable<Assembly> assemblies)
        {
            var schemaTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                schemaTypes.AddRange(assembly.GetTypes().Where(type => type.GetXmlDocTypeNodeWithJSchema() != null));
            }

            return schemaTypes;
        }

        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsArrayType(this Type type)
        {
            return type.IsGenericType 
                && (  type.GetGenericTypeDefinition() == typeof(List<>) 
                   || type.GetGenericTypeDefinition() == typeof(IList<>)
                   || type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                   );
        }
    }
}