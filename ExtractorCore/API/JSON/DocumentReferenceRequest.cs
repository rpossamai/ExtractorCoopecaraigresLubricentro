using Newtonsoft.Json;
using System;

namespace ExtractorFacturero.Core.API.JSON
{
    public class DocumentReferenceRequest
    {
        [JsonProperty(PropertyName = "clave")]
        public string Key { get; set; }

        [JsonIgnore]
        public DateTime IssuedAt { private get; set; }

        [JsonProperty(PropertyName = "fecha_emision")]
        public string IssuedAtDate
        {
            get
            {
                return this.IssuedAt.ToString("yyyy-MM-dd");
            }
        }

        [JsonProperty(PropertyName = "hora_emision")]
        public string IssuedAtTime
        {
            get
            {
                return this.IssuedAt.ToString("HH:mm:ss");
            }
        }

        [JsonProperty(PropertyName = "codigo_tipo")]
        public string TypeCode { get; set; }

        [JsonProperty(PropertyName = "codigo_accion")]
        public string ActionCode { get; set; }

        [JsonProperty(PropertyName = "razon")]
        public string Reason { get; set; }
    }
}
