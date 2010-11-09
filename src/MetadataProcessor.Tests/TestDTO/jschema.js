var schema = {
    "id": "JSchemaDTO",
    "description": "this is a description of JSchemaDTO\n             see http://tools.ietf.org/html/draft-zyp-json-schema-02",
    "type": "object",
    "properties": {
        "TestEnumProperty": {
            "description": "this is a description of JSchemaDTO.TestEnumProperty",
            "type": {
                "$ref": "#.properties.TestEnum"
            }
        },
        "IntProperty": {
            "description": "this is a description of JSchemaDTO.IntProperty",
            "title": "This provides a short description of the instance property.  The value must be a string. Full description is in xml-doc summary node",
            "maximumCanEqual": false,
            "minimumCanEqual": true,
            "optional": false,
            "type": "integer",
            "demoValue": 100,
            "default": -1,
            "minimum": 0,
            "maximum": 36
        },
        "UintProperty": {
            "description": "this is a description of JSchemaDTO.UintProperty",
            "type": "integer",
            "demoValue": 100,
            "default": -1,
            "minimum": 0,
            "maximum": 36
        },
        "LongProperty": {
            "description": "this is a description of JSchemaDTO.LongProperty",
            "type": "integer",
            "demoValue": 100,
            "default": -1,
            "minimum": 0,
            "maximum": 36
        },
        "UlongProperty": {
            "description": "this is a description of JSchemaDTO.UlongProperty",
            "type": "integer",
            "demoValue": 100,
            "default": -1,
            "minimum": 0,
            "maximum": 36
        },
        "FloatProperty": {
            "description": "this is a description of JSchemaDTO.FloatProperty",
            "type": "number",
            "demoValue": 100.0,
            "default": -1.5,
            "minimum": 0.0,
            "maximum": 36.0
        },
        "DoubleProperty": {
            "description": "this is a description of JSchemaDTO.DoubleProperty",
            "type": "number",
            "demoValue": 100.0,
            "default": -1.5,
            "minimum": 0.0,
            "maximum": 36.0
        },
        "DecimalProperty": {
            "description": "this is a description of JSchemaDTO.DecimalProperty",
            "type": "number",
            "demoValue": 100.0,
            "default": -1.5,
            "minimum": 0.0,
            "maximum": 36.0
        },
        "CharProperty": {
            "description": "this is a description of JSchemaDTO.CharProperty",
            "type": "string"
        },
        "StringProperty": {
            "description": "this is a description of JSchemaDTO.StringProperty",
            "title": "This provides a short description of the instance property.  The value must be a string. Full description is in xml-doc summary node",
            "format": "guid",
            "contentEncoding": "text/plain",
            "pattern": "^(([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12})$",
            "minLength": 36,
            "maxLength": 36,
            "optional": false,
            "type": "string",
            "demoValue": "D2FF3E4D-01EA-4741-86F0-437C919B5559",
            "default": "a default value if instance property is null"
        },
        "BoolProperty": {
            "description": "this is a description of JSchemaDTO.BoolProperty",
            "optional": true,
            "type": "boolean",
            "default": true
        },
        "ObjectProperty": {
            "description": "this is a description of JSchemaDTO.ObjectProperty",
            "type": "string",
            "default": "true"
        },
        "JSchemaDTOProperty": {
            "description": "this is a description of JSchemaDTO.JSchemaDTOProperty",
            "type": "string"
        },
        "IListJSchemaDTOProperty": {
            "description": "this is a description of JSchemaDTO.IListJSchemaDTOProperty",
            "type": "string"
        },
        "ListJSchemaDTOProperty": {
            "description": "this is a description of JSchemaDTO.ListJSchemaDTOProperty",
            "type": "string"
        },
        "ICollectionJSchemaDTOProperty": {
            "description": "this is a description of JSchemaDTO.ICollectionJSchemaDTOProperty",
            "type": "string"
        },
        "IEnumerableJSchemaDTOProperty": {
            "description": "this is a description of JSchemaDTO.IEnumerableJSchemaDTOProperty",
            "type": "string"
        }
    }
}