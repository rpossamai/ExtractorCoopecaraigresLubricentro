using Newtonsoft.Json;

namespace ExtractorFacturero.Core.API.JSON
{
    public class DocumentRecipientDocumentRequest
    {
        [JsonProperty(PropertyName = "codigo_tipo")]
        public string TypeCode { get; set; }

        [JsonProperty(PropertyName = "numero")]
        public string Number { get; set; }
    }
}
