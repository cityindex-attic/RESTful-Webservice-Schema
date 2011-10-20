var foo = 
{
    "services": {
        "Service1": {
            "description": "foo",
            "target": "session",
            "uriTemplate": "/",
            "contentType": "application/json",
            "responseContentType": "application/json",
            "transport": "POST",
            "envelope": "JSON",
            "returns": {
                "$ref": "#.CreateSessionResponseDTO"
            },
            "group": "Authentication",
            "throttleScope": "data",
            "parameters": [
        {
            "type": "string",
            "name": "UserName",
            "description": "Username is case sensitive",
            "demoValue": "CC735158",
            "minLength": 6,
            "maxLength": 20
        },
        {
            "type": "string",
            "name": "Password",
            "description": "Password is case sensitive",
            "demoValue": "password",
            "minLength": 6,
            "maxLength": 20
        }
      ]
        }
    }
}