using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ExtractorFacturero.Core.API.JSON
{
    public class DocumentRequest
    {
        [JsonProperty(PropertyName = "encabezado")]
        public DocumentHeaderRequest Header { get; set; }

        [JsonProperty(PropertyName = "receptor")]
        public DocumentRecipientRequest Recipient { get; set; }

        [JsonProperty(PropertyName = "numero_control")]
        public string ControlNumber { get; set; }

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

        [JsonProperty(PropertyName = "codigo_condicion_venta")]
        public string SaleConditionCode { get; set; }
		
        [JsonProperty(PropertyName = "plazo_credito_en_dias")]
        public string CreditTermInDays { get; set; }

        [JsonProperty(PropertyName = "observaciones")]
        public string Remarks { get; set; }

        [JsonProperty(PropertyName = "informacion_pagos")]
        public List<DocumentPaymentInformationRequest> PaymentsInformation { get; set; }

        [JsonProperty(PropertyName = "elementos")]
        public List<DocumentItemRequest> Items { get; set; }

        [JsonProperty(PropertyName = "documentos_referencia")]
        public List<DocumentReferenceRequest> References { get; set; }

        [JsonProperty(PropertyName = "resumen")]
        public DocumentSummaryRequest Summary { get; set; }

        [JsonProperty(PropertyName = "extras")]
        public Dictionary<string, string> Extras { get; set; }
    }
}
