using System.ServiceModel.Channels;

namespace RESTWebServices.XDRSupport
{
    /// <summary>
    /// We have to allow for incoming content types other than JSON to be mapped to JSON
    /// to support IE XDR and simple POST to avoid pre-flight options 
    /// </summary>
    public class JsonContentTypeMapper : WebContentTypeMapper
    {

        public override WebContentFormat GetMessageFormatForContentType(string contentType)
        {

            contentType = (contentType + "").ToLower();
            // WTF?! IE XDR is sending "application/octet-stream" - supposed to be text/plain
            // will accept it but also allow for the spec of text/plain))
            if ((contentType == "application/octet-stream" || contentType.Contains("text/plain")))
            {

                return WebContentFormat.Json;
            }
            else
            {
                return WebContentFormat.Default;
            }
        }
    }
}