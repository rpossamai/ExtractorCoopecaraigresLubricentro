using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ExtractorFacturero.Core.API.JSON
{
    public class DocumentItemRequest
    {
        [JsonProperty(PropertyName = "posicion")]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "cantidad")]
        public Decimal Quantity { get; set; }

        [JsonProperty(PropertyName = "codigos")]
        public List<DocumentItemCodeRequest> Codes { get; set; }

        [JsonProperty(PropertyName = "codigo_cabys")]
        public string CabysCode { get; set; }

        [JsonProperty(PropertyName = "codigo_unidad_medida")]
        public string MeasurementUnitCode { get; set; }

        [JsonProperty(PropertyName = "descripcion")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "precio_unitario")]
        public Decimal UnitPrice { get; set; }

        [JsonProperty(PropertyName = "monto_total")]
        public Decimal TotalAmount { get; set; }

        [JsonProperty(PropertyName = "monto_descuento")]
        public Decimal? DiscountAmount { get; set; }

        [JsonProperty(PropertyName = "descripcion_descuento")]
        public string DiscountDescription { get; set; }

        [JsonProperty(PropertyName = "sub_total")]
        public Decimal SubTotal { get; set; }

        [JsonProperty(PropertyName = "impuestos")]
        public List<DocumentItemTaxRequest> Taxes { get; set; }

        [JsonProperty(PropertyName = "monto_total_linea")]
        public Decimal ItemTotalAmount { get; set; }

        [JsonProperty(PropertyName = "impuesto_asumido_emisor_fabrica")]
        public Decimal TaxAssumedIssuerFactory { get; set; }

        [JsonProperty(PropertyName = "base_imponible")]
        public Decimal? TaxableBase { get; set; }

        [JsonProperty(PropertyName = "numero_vin_o_serie")]
        public string VinOrSerialNumber { get; set; }

        [JsonProperty(PropertyName = "codigo_tipo_transaccion")]
        public string TransactionTypeCode { get; set; }

        [JsonProperty(PropertyName = "codigo_iva_cobrado_fabrica")]
        public string FactoryVatCode { get; set; }

        [JsonProperty(PropertyName = "registro_medicamento")]
        public string MedicineRegistration { get; set; }

        [JsonProperty(PropertyName = "codigo_forma_farmaceutica")]
        public string PharmaceuticalFormCode { get; set; }

        [JsonProperty(PropertyName = "descuento")]
        public DocumentItemDiscountRequest Discount { get; set; }

        [JsonProperty(PropertyName = "impuesto_neto")]
        public Decimal NetTaxAmount { get; set; }
    }
}
