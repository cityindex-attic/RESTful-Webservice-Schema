using System.Collections.Generic;

namespace JsonSchemaGeneration
{
    public class MetadataValidationResult
    {
        private readonly List<MetadataGenerationSuccess> _metadataGenerationSuccesses = new List<MetadataGenerationSuccess>();
        public List<MetadataGenerationSuccess> MetadataGenerationSuccesses
        {
            get { return _metadataGenerationSuccesses; }
        }
        public void AddMetadataGenerationSuccess(MetadataGenerationSuccess success)
        {
            _metadataGenerationSuccesses.Add(success);
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
    }
}