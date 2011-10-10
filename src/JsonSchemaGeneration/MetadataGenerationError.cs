using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsonSchemaGeneration
{
    public enum ErrorType
    {
        JsonSchema,
        SMD
    } ;

    public class MetadataGenerationError
    {
        public MetadataGenerationError(ErrorType errorType, Type type, string errorReason, string sugestionedSolution)
        {
            ErrorType = errorType;
            Type = type;
            ErrorReason = errorReason;
            SugestionedSolution = sugestionedSolution;
        }

        public ErrorType ErrorType { get; private set; }
        public Type Type { get; private set; }
        public string ErrorReason { get; private set; }
        public string SugestionedSolution { get; private set; }
    }
}
