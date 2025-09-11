using Newtonsoft.Json;

namespace ExtractorFacturero.Core.API.JSON
{
    public class DocumentItemCodeRequest
    {
        [JsonProperty(PropertyName = "tipo")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "codigo")]
        public string Code { get; set; }
    }
}
