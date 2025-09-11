using Newtonsoft.Json;
using System;

namespace ExtractorFacturero.Core.API.JSON
{
    public class DocumentPaymentInformationRequest
    {
        [JsonProperty(PropertyName = "codigo_tipo")]
        public string TypeCode { get; set; }

        [JsonProperty(PropertyName = "monto")]
        public Decimal Amount { get; set; }
    }
}
