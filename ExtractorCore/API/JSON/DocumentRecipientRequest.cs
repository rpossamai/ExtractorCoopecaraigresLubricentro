using Newtonsoft.Json;

namespace ExtractorFacturero.Core.API.JSON
{
    public class DocumentRecipientRequest
    {
        [JsonProperty(PropertyName = "nombre")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "identificacion")]
        public DocumentRecipientDocumentRequest Document { get; set; }

        [JsonProperty(PropertyName = "correo_electronico")]
        public string EmailAddress { get; set; }
    }
}
