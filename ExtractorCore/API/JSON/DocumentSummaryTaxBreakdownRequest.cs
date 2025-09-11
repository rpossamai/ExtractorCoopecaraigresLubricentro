using Newtonsoft.Json;
using System;

namespace ExtractorFacturero.Core.API.JSON
{
    public class DocumentSummaryTaxBreakdownRequest
    {
        [JsonProperty(PropertyName = "codigo_tipo")]
        public string TypeCode { get; set; }

        [JsonProperty(PropertyName = "codigo_tarifa_iva")]
        public string VatTaxRateCode { get; set; }

        [JsonProperty(PropertyName = "monto")]
        public Decimal Amount { get; set; }
    }
}