using System.Collections.Generic;

namespace JsonSchemaGeneration
{
    public class MetadataGenerationResult
    {
        public bool HasErrors
        {
            get { return _metadataGenerationErrors.Count > 0; }
        }

        private readonly List<MetadataGenerationError> _metadataGenerationErrors = new List<MetadataGenerationError>();
        public List<MetadataGenerationError> MetadataGenerationErrors
        {
            get { return _metadataGenerationErrors; }
        }
        public void AddMetadataGenerationError(MetadataGenerationError error)
        {
           _metadataGenerationErrors.Add(error);
        }

        public string JsonSchema { get; set; }
        public string SMD { get; set; }
    }
}