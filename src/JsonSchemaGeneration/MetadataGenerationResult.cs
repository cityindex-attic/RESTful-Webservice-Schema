using System.Collections.Generic;

namespace JsonSchemaGeneration
{
    public class MetadataGenerationResult
    {
        public bool HasErrors
        {
            get { return _metadataGenerationErrors.Count > 0; }
        }

        private List<MetadataGenerationError> _metadataGenerationErrors = new List<MetadataGenerationError>();
        public List<MetadataGenerationError> MetadataGenerationErrors
        {
            get { return _metadataGenerationErrors; }
            set { _metadataGenerationErrors = value; }
        }

        private List<MetadataGenerationSuccess> _metadataGenerationSuccesses = new List<MetadataGenerationSuccess>();
        public List<MetadataGenerationSuccess> MetadataGenerationSuccesses
        {
            get { return _metadataGenerationSuccesses; }
            set { _metadataGenerationSuccesses = value; }
        }

        public string JsonSchema { get; set; }
        public string SMD { get; set; }
    }
}