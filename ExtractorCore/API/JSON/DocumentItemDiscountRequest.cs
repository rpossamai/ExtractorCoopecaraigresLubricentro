using Newtonsoft.Json;
using System;

namespace ExtractorFacturero.Core.API.JSON
{
    public class DocumentItemDiscountRequest
    {
        [JsonProperty(PropertyName = "codigo_tipo")]
        public string TypeCode { get; set; }

        [JsonProperty(PropertyName = "monto")]
        public Decimal Amount { get; set; }

        [JsonProperty(PropertyName = "descripcion")]
        public string Description { get; set; }

        /*[JsonProperty(PropertyName = "descripcion_tipo")]
        public string DescriptionType { get; set; }*/
    }
}