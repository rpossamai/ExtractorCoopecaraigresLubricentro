using ExtractorFacturero.Core.Data.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ExtractorFacturero.Core.Data
{
    public class DataBase : IDisposable
    {
        private const string R_ALL_PENDING_DOCUMENTS = "SELECT TOP 10 * FROM FE_DOCUPEND";
        private const string R_ALL_PENDING_NOTES = "SELECT * FROM FE_NOTEPEND";
        private const string R_ALL_UNACKED_DOCUMENTS = "SELECT FE_CONTROL_NUMBER FROM FE_DOCUTRIB WHERE FE_HTTP_RESPCODE IS NULL OR FE_HTTP_RESPCODE NOT IN (200, 202, 400, 409)";
        private const string R_ALL_STATUSLESS_TRIBUNET_INQUIRIES = "SELECT FE_CONTROL_NUMBER, FE_KEY FROM FE_DOCUTRIB WHERE (FE_STATUS IS NULL OR FE_STATUS = 'procesando') AND FE_HTTP_RESPCODE = 202";
        private const string R_TRAMADO_ITEMS_BY_CONTROL_NUMBER = "SELECT * FROM TRAMADO WHERE CONSECUTIVO = @controlNumber";
        private const string R_LLANTAS_ITEMS_BY_CONTROL_NUMBER = "SELECT * FROM LLANTAS WHERE CONSECUTIVO = @controlNumber";
        private const string C_DOCUMENT_TRIBUNET_CONTROL = "INSERT INTO FE_DOCUTRIB(FE_CONTROL_NUMBER, FE_ISSUED_AT, FE_TYPE_CODE) VALUES(@controlNumber, @issuedAt, @typeCode)";
        private const string D_DOCUMENT_TRIBUNET_CONTROL = "DELETE FROM FE_DOCUTRIB WHERE FE_CONTROL_NUMBER = @controlNumber";
        private const string U_DOCUMENT_TRIBUNET_CONTROL_STATUS = "UPDATE FE_DOCUTRIB SET FE_HTTP_RESPCODE = @httpStatusCode WHERE FE_CONTROL_NUMBER = @controlNumber";
        private const string U_DOCUMENT_TRIBUNET_CONTROL_KEY_NUMBER = "UPDATE FE_DOCUTRIB SET FE_KEY = @documentKey, FE_NUMBER = @documentNumber WHERE FE_CONTROL_NUMBER = @controlNumber";
        private const string U_DOCUMENT_TRIBUNET_CONTROL_STATUS_KEY_NUMBER = "UPDATE FE_DOCUTRIB SET FE_HTTP_RESPCODE = @httpStatusCode, FE_KEY = @documentKey, FE_NUMBER = @documentNumber WHERE FE_CONTROL_NUMBER = @controlNumber";
        private const string U_DOCUMENT_TRIBUNET_CONTROL_STATUS_MESSAGE = "UPDATE FE_DOCUTRIB SET FE_STATUS = @responseStatus, FE_MESSAGE = @responseMessage WHERE FE_CONTROL_NUMBER = @controlNumber";
        private readonly string connectionString;
        private readonly SqlConnection conn;

        public DataBase()
        {
            try
            {
                this.connectionString = ConfigurationManager.ConnectionStrings["SISTEMA"].ConnectionString;
                //Console.WriteLine($"[DEBUG] Connection string obtenido: {connectionString}");
                this.conn = new SqlConnection(connectionString);
                //Console.WriteLine("[DEBUG] SqlConnection creado exitosamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error en constructor DataBase: {ex.Message}");
                throw;
            }
        }

        protected void Open()
        {
            this.conn.Open();
        }

        protected void OpenIfNotYet()
        {
            try
            {
                //Console.WriteLine($"[DEBUG] Estado actual de conexión: {conn.State}");
                //Console.WriteLine($"[DEBUG] Connection String: {connectionString}");
                
                if (conn.State != ConnectionState.Open)
                {
                    //Console.WriteLine("[DEBUG] Intentando abrir conexión...");
                    this.conn.Open();
                    //Console.WriteLine($"[DEBUG] Conexión abierta exitosamente. Estado: {conn.State}");
                }
                else
                {
                    //Console.WriteLine("[DEBUG] La conexión ya estaba abierta");
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"[ERROR] Error SQL al abrir conexión: {sqlEx.Message}");
                Console.WriteLine($"[ERROR] Número de error SQL: {sqlEx.Number}");
                Console.WriteLine($"[ERROR] Severidad: {sqlEx.Class}");
                Console.WriteLine($"[ERROR] Estado: {sqlEx.State}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error general al abrir conexión: {ex.Message}");
                Console.WriteLine($"[ERROR] Tipo de excepción: {ex.GetType().Name}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public void DeleteDocumentTribunetControl(string controlNumber)
        {
            OpenIfNotYet();

            using (SqlCommand cmd = new SqlCommand(D_DOCUMENT_TRIBUNET_CONTROL, conn))
            {
                cmd.Parameters.AddWithValue("@controlNumber", controlNumber);

                cmd.ExecuteNonQuery();
            }
        }

        public void CreateDocumentTribunetControl(string controlNumber, DateTime issuedAt, string typeCode)
        {
            OpenIfNotYet();

            using (SqlCommand cmd = new SqlCommand(C_DOCUMENT_TRIBUNET_CONTROL, conn))
            {
                cmd.Parameters.AddWithValue("@controlNumber", controlNumber);
                cmd.Parameters.AddWithValue("@issuedAt", issuedAt);
                cmd.Parameters.AddWithValue("@typeCode", typeCode);

                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateDocumentTribunetControlWithStatus(string controlNumber, int httpStatusCode)
        {
            OpenIfNotYet();

            using (SqlCommand cmd = new SqlCommand(U_DOCUMENT_TRIBUNET_CONTROL_STATUS, conn))
            {
                cmd.Parameters.AddWithValue("@controlNumber", controlNumber);
                cmd.Parameters.AddWithValue("@httpStatusCode", httpStatusCode);

                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateDocumentTribunetControlWithKeyAndNumber(string controlNumber, string documentKey, string documentNumber)
        {
            OpenIfNotYet();

            using (SqlCommand cmd = new SqlCommand(U_DOCUMENT_TRIBUNET_CONTROL_KEY_NUMBER, conn))
            {
                cmd.Parameters.AddWithValue("@controlNumber", controlNumber);
                cmd.Parameters.AddWithValue("@documentKey", documentKey);
                cmd.Parameters.AddWithValue("@documentNumber", documentNumber);

                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateDocumentTribunetControlWithStatusAndKeyAndNumber(string controlNumber, int httpStatusCode, string documentKey, string documentNumber)
        {
            OpenIfNotYet();

            using (SqlCommand cmd = new SqlCommand(U_DOCUMENT_TRIBUNET_CONTROL_STATUS_KEY_NUMBER, conn))
            {
                cmd.Parameters.AddWithValue("@controlNumber", controlNumber);
                cmd.Parameters.AddWithValue("@httpStatusCode", httpStatusCode);
                cmd.Parameters.AddWithValue("@documentKey", documentKey);
                cmd.Parameters.AddWithValue("@documentNumber", documentNumber);

                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateDocumentTribunetControlWithStatusAndMessage(string controlNumber, string responseStatus, string responseMessage)
        {
            OpenIfNotYet();

            using (SqlCommand cmd = new SqlCommand(U_DOCUMENT_TRIBUNET_CONTROL_STATUS_MESSAGE, conn))
            {
                cmd.Parameters.AddWithValue("@controlNumber", controlNumber);
                cmd.Parameters.AddWithValue("@responseStatus", responseStatus);
                cmd.Parameters.AddWithValue("@responseMessage", !string.IsNullOrEmpty(responseMessage) ? (object)responseMessage : DBNull.Value);
                
                cmd.ExecuteNonQuery();
            }
        }

        private List<DocumentDTO> ReadDocumentDTOResultSet(SqlCommand cmd, bool isPendingNotes = false)
        {
            //Console.WriteLine($"[DEBUG] Ejecutando ReadDocumentDTOResultSet, timeout: {cmd.CommandTimeout}s");
            List<DocumentDTO> documents = new List<DocumentDTO>();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                int FE_BRANCH_CODE = reader.GetOrdinal("FE_BRANCH_CODE"),
                    FE_BRANCH_TERMINAL_CODE = reader.GetOrdinal("FE_BRANCH_TERMINAL_CODE"),
                    FE_CONTROL_NUMBER = reader.GetOrdinal("FE_CONTROL_NUMBER"),
                    FE_ISSUED_AT = reader.GetOrdinal("FE_ISSUED_AT"),
                    FE_TYPE_CODE = reader.GetOrdinal("FE_TYPE_CODE"),
                    FE_SALE_CONDITION_CODE = reader.GetOrdinal("FE_SALE_CONDITION_CODE"),
                    FE_RECIPIENT_NAME = reader.GetOrdinal("FE_RECIPIENT_NAME"),
                    FE_RECIPIENT_DOCUMENT_NUMBER = reader.GetOrdinal("FE_RECIPIENT_DOCUMENT_NUMBER"),
                    FE_RECIPIENT_EMAIL_ADDRESS = reader.GetOrdinal("FE_RECIPIENT_EMAIL_ADDRESS"),
                    FE_REMARKS = reader.GetOrdinal("FE_REMARKS"),
                    FE_EXTRA_PLATE = reader.GetOrdinal("FE_EXTRA_PLATE");

                while (reader.Read())
                {
                    DocumentDTO document = new DocumentDTO();
                    document.BranchCode = reader.GetString(FE_BRANCH_CODE);
                    document.BranchTerminalCode = reader.GetString(FE_BRANCH_TERMINAL_CODE);
                    document.ControlNumber = reader.GetValue(FE_CONTROL_NUMBER).ToString();
                    document.IssuedAt = reader.GetDateTime(FE_ISSUED_AT);
                    document.TypeCode = reader.GetString(FE_TYPE_CODE);
                    document.SaleConditionCode = reader.GetString(FE_SALE_CONDITION_CODE);
                    document.RecipientName = !reader.IsDBNull(FE_RECIPIENT_NAME) ? reader.GetString(FE_RECIPIENT_NAME) : null;
                    document.RecipientDocumentNumber = !reader.IsDBNull(FE_RECIPIENT_DOCUMENT_NUMBER) ? reader.GetString(FE_RECIPIENT_DOCUMENT_NUMBER) : null;
                    document.RecipientEmailAddress = !reader.IsDBNull(FE_RECIPIENT_EMAIL_ADDRESS) ? reader.GetString(FE_RECIPIENT_EMAIL_ADDRESS) : null;
                    document.CurrencyCode = "CRC";
                    document.Remarks = !reader.IsDBNull(FE_REMARKS) ? reader.GetString(FE_REMARKS) : null;
                    document.ExtraLicensePlate = !reader.IsDBNull(FE_EXTRA_PLATE) ? reader.GetString(FE_EXTRA_PLATE) : null;
                    document.Items = FindAllDocumentItemsByControlNumber(document.ControlNumber.Substring(2), document.ControlNumber.StartsWith("LL"));
                    documents.Add(document);
                }
            }
            return documents;
        }

        public List<DocumentDTO> FindAllPendingNotes()
        {
            OpenIfNotYet();

            using (SqlCommand cmd = new SqlCommand(R_ALL_PENDING_NOTES, conn))
            { 
                return ReadDocumentDTOResultSet(cmd, true);
            }
        }

        public List<DocumentDTO> FindAllPendingDocuments()
        {
            try
            {
                //Console.WriteLine("[DEBUG] Iniciando FindAllPendingDocuments()");
                OpenIfNotYet();
                //Console.WriteLine("[DEBUG] Conexión abierta, ejecutando consulta SQL");

                using (SqlCommand cmd = new SqlCommand(R_ALL_PENDING_DOCUMENTS, conn))
                {
                    cmd.CommandTimeout = 240; // 2 minutos de timeout
                    //Console.WriteLine($"[DEBUG] Ejecutando query: {R_ALL_PENDING_DOCUMENTS} (Timeout: {cmd.CommandTimeout}s)");
                    
                    var startTime = DateTime.Now;
                    var result = ReadDocumentDTOResultSet(cmd, false);
                    var endTime = DateTime.Now;
                    
                    //Console.WriteLine($"[DEBUG] Query ejecutada exitosamente en {(endTime - startTime).TotalSeconds:F2} segundos, {result.Count} documentos encontrados");
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error en FindAllPendingDocuments: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public List<DocumentItemDTO> FindAllDocumentItemsByControlNumber(string controlNumber, bool llantas)
        {
            OpenIfNotYet();

            List<DocumentItemDTO> documents = new List<DocumentItemDTO>();
            using (SqlCommand cmd = new SqlCommand(llantas ? R_LLANTAS_ITEMS_BY_CONTROL_NUMBER : R_TRAMADO_ITEMS_BY_CONTROL_NUMBER, conn))
            {
                cmd.Parameters.AddWithValue("@controlNumber", int.Parse(controlNumber));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int CODIGO = reader.GetOrdinal("CODIGO"),
                        DESCRIPCION = reader.GetOrdinal("DESCRIPCION"),
                        PRECIO = reader.GetOrdinal("PRECIO");

                    int position = 1;
                    while (reader.Read())
                    {
                        var subTotal = Math.Round(reader.GetDecimal(PRECIO) / 1.13m, 2);
                        var taxAmount = reader.GetDecimal(PRECIO) - subTotal;
                        var taxableBase = subTotal;
                        
                        documents.Add(new DocumentItemDTO
                        {
                            Position = position++,
                            ItemCode = reader.GetString(CODIGO),
                            Description = reader.GetString(DESCRIPCION),
                            Quantity = 1,
                            MeasurementUnitCode = "Sp",
                            UnitPrice = subTotal,
                            TotalAmount = subTotal,
                            SubTotal = subTotal,
                            TaxRate = 13,
                            TaxAmount = taxAmount,
                            ItemTotalAmount = reader.GetDecimal(PRECIO),
                            TaxAssumedIssuerFactory = 0,
                            TaxableBase = taxableBase,
                            VinOrSerialNumber = null,
                            TransactionTypeCode = null,
                            FactoryVatCode = null,
                            MedicineRegistration = null,
                            PharmaceuticalFormCode = null,
                            Discount = null,
                            NetTaxAmount = taxAmount // NetTaxAmount = TaxAmount - TaxExempt - TaxAssumedIssuerFactory
                        });
                    }
                }
            }
            return documents;
        }

        public Dictionary<string, string> FindAllStatuslessTribunetInquiries()
        {
            OpenIfNotYet();

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            using (SqlCommand cmd = new SqlCommand(R_ALL_STATUSLESS_TRIBUNET_INQUIRIES, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int FE_CONTROL_NUMBER = reader.GetOrdinal("FE_CONTROL_NUMBER"),
                        FE_KEY = reader.GetOrdinal("FE_KEY");

                    while (reader.Read())
                    {
                        dictionary.Add(reader.GetValue(FE_CONTROL_NUMBER).ToString(), reader.GetString(FE_KEY));
                    }
                }
            }

            return dictionary;
        }

        public List<string> FindAllUnackedDocuments()
        {
            OpenIfNotYet();

            List<string> list = new List<string>();
            using (SqlCommand cmd = new SqlCommand(R_ALL_UNACKED_DOCUMENTS, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int FE_CONTROL_NUMBER = reader.GetOrdinal("FE_CONTROL_NUMBER");

                    while (reader.Read())
                    {
                        list.Add(reader.GetValue(FE_CONTROL_NUMBER).ToString());
                    }
                }
            }

            return list;
        }

        public void Dispose()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
}
