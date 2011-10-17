using System;

namespace JsonSchemaGeneration
{
    public class MetadataGenerationSuccess
    {
        public MetadataGenerationSuccess(MetadataType metadataType, Type type)
        {
            MetadataType = metadataType;
            Type = type;
        }

        public MetadataType MetadataType { get; private set; }
        public Type Type { get; private set; }
    }
}