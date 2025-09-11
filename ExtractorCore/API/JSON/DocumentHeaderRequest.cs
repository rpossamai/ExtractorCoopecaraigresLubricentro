using Newtonsoft.Json;

namespace ExtractorFacturero.Core.API.JSON
{
    public class DocumentHeaderRequest
    {
        [JsonProperty(PropertyName = "codigo_sucursal")]
        public string BranchCode { get; set; }

        [JsonProperty(PropertyName = "codigo_terminal")]
        public string BranchTerminalCode { get; set; }

        [JsonProperty(PropertyName = "situacion_documento")]
        public int DocumentKind { get; set; }

        [JsonProperty(PropertyName = "codigo_seguridad")]
        public string SecurityCode { get; set; }

        [JsonProperty(PropertyName = "codigo_actividad")]
        public string EconomicActivityCode { get; set; }
    }
}
