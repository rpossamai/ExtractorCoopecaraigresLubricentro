using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ExtractorFacturero.Core.API.JSON
{
    public class DocumentSummaryRequest
    {
        [JsonProperty(PropertyName = "codigo_moneda")]
        public string CurrencyCode { get; set; }

        [JsonProperty(PropertyName = "total_servicios_gravados")]
        public Decimal TotalTaxedServices { get; set; }

        [JsonProperty(PropertyName = "total_mercancias_gravadas")]
        public Decimal TotalTaxedGoods { get; set; }

        [JsonProperty(PropertyName = "total_gravado")]
        public Decimal TotalTaxed { get; set; }

        [JsonProperty(PropertyName = "total_mercancias_exentas")]
        public Decimal TotalExemptedGoods { get; set; }

        [JsonProperty(PropertyName = "total_servicios_exentos")]
        public Decimal TotalExemptedServices { get; set; }

        [JsonProperty(PropertyName = "total_exento")]
        public Decimal TotalExempted { get; set; }

        [JsonProperty(PropertyName = "total_venta")]
        public Decimal TotalSale { get; set; }

        [JsonProperty(PropertyName = "total_descuentos")]
        public Decimal TotalDiscounts { get; set; }

        [JsonProperty(PropertyName = "total_venta_neta")]
        public Decimal TotalNetSale { get; set; }

        [JsonProperty(PropertyName = "total_impuesto")]
        public Decimal TotalTax { get; set; }

        [JsonProperty(PropertyName = "total_comprobante")]
        public Decimal Total { get; set; }

        [JsonProperty(PropertyName = "total_servicios_no_sujetos")]
        public Decimal TotalUntaxedServices { get; set; }

        [JsonProperty(PropertyName = "total_mercancias_no_sujetas")]
        public Decimal TotalUntaxedGoods { get; set; }

        [JsonProperty(PropertyName = "total_no_sujeto")]
        public Decimal TotalUntaxed { get; set; }

        [JsonProperty(PropertyName = "total_impuesto_asumido_emisor_fabrica")]
        public Decimal TotalTaxAssumedIssuerFactory { get; set; }

        [JsonProperty(PropertyName = "total_desglose_impuesto")]
        public List<DocumentSummaryTaxBreakdownRequest> TaxBreakdown { get; set; }
    }
}
