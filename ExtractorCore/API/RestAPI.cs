using ExtractorFacturero.Core.API.JSON;
using ExtractorFacturero.Core.API.Serialization;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;
using System;
using System.Configuration;

namespace ExtractorFacturero.Core.API
{
    public class RestAPI
    {
        private const string P_DOCUMENTOS = "/documentos";
        private const string P_TRIBUNET_SOLICITUDES_EMISOR_RESPUESTA = "/tribunet/solicitudes/emisor/{0}/respuesta";
        private const string P_DOCUMENTOS__PAGEABLE = "/documentos?fecha_hora_posteo={0}&fecha_hora_posteo={1}&numero_control={2}&sort=fecha_hora_posteo";
        private RestClient _restClient;
        private string _endpointUrl;
        private string _username;
        private string _password;

        public RestAPI()
        {
            this._endpointUrl = ConfigurationManager.AppSettings["EndpointURL"];
            this._username = ConfigurationManager.AppSettings["EndpointUsername"];
            this._password = ConfigurationManager.AppSettings["EndpointPassword"];
            this._restClient = new RestClient(this._endpointUrl);
            this._restClient.Authenticator = (IAuthenticator)new HttpBasicAuthenticator(this._username, this._password);
        }

        public IRestResponse PostDocument(DocumentRequest document)
        {
            RestRequest restRequest = new RestRequest("/documentos", (Method)1);
            restRequest.JsonSerializer = (ISerializer)NewtonsoftJsonSerializer.Default;
            restRequest.AddJsonBody((object)document);
            return this._restClient.Execute((IRestRequest)restRequest);
        }

        public IRestResponse GetPagedDocumentsByControlNumber(string controlNumber)
        {
            return this._restClient.Execute((IRestRequest)new RestRequest(string.Format("/documentos?fecha_hora_posteo={0}&fecha_hora_posteo={1}&numero_control={2}&sort=fecha_hora_posteo", (object)DateTime.Now.AddMonths(-1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"), (object)DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"), (object)controlNumber), (Method)0));
        }

        public IRestResponse GetTribunetInquiryResponse(string key)
        {
            return this._restClient.Execute((IRestRequest)new RestRequest(string.Format("/tribunet/solicitudes/emisor/{0}/respuesta", (object)key), (Method)0));
        }
    }
}
