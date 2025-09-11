using ExtractorFacturero.Core.API;
using ExtractorFacturero.Core.API.JSON;
using ExtractorFacturero.Core.Data;
using ExtractorFacturero.Core.Data.DTO;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ExtractorFacturero.Core
{
    public class SyncOperations
    {
        private static readonly RestAPI API = new RestAPI();
        private static string eventSourceName = "Facturero.cr";
        private static string logName = "Application";

        private static void WriteEventLogEntry(string fmt, params object[] args)
        {
            new EventLog()
            {
                Source = SyncOperations.eventSourceName,
                Log = SyncOperations.logName
            }.WriteEntry(string.Format(fmt, args));
        }

        public void UpdateStatuslessInquiries()
        {
            using (DataBase db = new DataBase())
            {
                Dictionary<string, string> tribunetInquiries = db.FindAllStatuslessTribunetInquiries();
                if (tribunetInquiries.Count > 0)
                {
                    WriteEventLogEntry("Se encontraron {0} solicitudes sin respuesta de Tribunet.", tribunetInquiries.Count);

                    StringBuilder sb = new StringBuilder();
                    foreach (var entry in tribunetInquiries)
                    {
                        var response = API.GetTribunetInquiryResponse(entry.Value);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            JObject jobject = JObject.Parse(response.Content);
                            string responseStatus = jobject["estado"].ToString();
                            string responseMessage = null;
                            if (jobject["mensaje"] != null)
                            {
                                responseMessage = jobject["mensaje"].ToString();
                            }

                            db.UpdateDocumentTribunetControlWithStatusAndMessage(entry.Key, responseStatus, responseMessage);
                            sb.AppendFormat("* Documento [{0}] con clave [{1}] actualizado con el estado [{2}].", entry.Key, entry.Value, responseStatus).AppendLine();
                        }
                    }

                    if (sb.Length > 0)
                    {
                        sb.Insert(0, "Se listan las solicitudes sin respuesta de Tribunet actualizadas:\n");

                        WriteEventLogEntry(sb.ToString());
                    }
                }
            }
        }

        public void UpdateUnackedDocuments()
        {
            using (DataBase db = new DataBase())
            {
                List<string> unackedDocuments = db.FindAllUnackedDocuments();
                if (unackedDocuments.Count > 0)
                {
                    WriteEventLogEntry("Se encontraron {0} envios sin respuesta HTTP desde Facturero.cr.", (object)unackedDocuments.Count);
                    
                    StringBuilder success = new StringBuilder();
                    StringBuilder error = new StringBuilder();
                    foreach (string controlNumber in unackedDocuments)
                    {
                        var response = API.GetPagedDocumentsByControlNumber(controlNumber);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            JArray jarray = (JArray)JObject.Parse(response.Content)["content"];
                            if (jarray != null && jarray.Count > 0)
                            {
                                for (int index = 0; index < jarray.Count; ++index)
                                {
                                    string documentKey = jarray[index]["clave"].ToString();
                                    string documentNumber = jarray[index]["numero"].ToString();

                                    db.UpdateDocumentTribunetControlWithStatusAndKeyAndNumber(controlNumber, (int)response.StatusCode, documentKey, documentNumber);
                                    success.AppendFormat("* Documento [{0}] actualizado con la clave [{1}].", controlNumber, documentKey).AppendLine();
                                }
                            }
                            else
                            {
                                db.DeleteDocumentTribunetControl(controlNumber);
                                error.AppendFormat("* Documento {0} eliminado.", (object)controlNumber).AppendLine();
                            }
                        }
                    }

                    if (success.Length > 0)
                    {
                        success.Insert(0, "Se listan los documentos sin respuesta HTTP actualizados:\n");
                        
                        WriteEventLogEntry(success.ToString());
                    }

                    if (error.Length > 0)
                    {
                        error.Insert(0, "Se listan los documentos sin respuesta HTTP eliminados:\n");

                        WriteEventLogEntry(error.ToString());
                    }
                }
            }
        }

        public void PostPendingNotes()
        {
            using (DataBase db = new DataBase())
            {
                List<DocumentDTO> pendingNotes = db.FindAllPendingNotes();
                if (pendingNotes.Count > 0)
                {
                    WriteEventLogEntry("Se encontraron {0} notas pendientes por enviar", pendingNotes.Count);
                    
                    PostDocuments(db, pendingNotes);
                }
            }
        }

        public void PostPendingDocuments()
        {
            using (DataBase db = new DataBase())
            {
                List<DocumentDTO> pendingDocuments = db.FindAllPendingDocuments();
                if (pendingDocuments.Count > 0)
                {
                    //WriteEventLogEntry("Se encontraron {0} documentos pendientes por enviar", pendingDocuments.Count);
                    
                    PostDocuments(db, pendingDocuments);
                }
            }
        }

        private void PostDocuments(DataBase db, List<DocumentDTO> pendingDocuments)
        {
            StringBuilder success = new StringBuilder();
            StringBuilder error = new StringBuilder();
            foreach (DocumentDTO pendingDocument in pendingDocuments)
            {
                db.CreateDocumentTribunetControl(pendingDocument.ControlNumber, pendingDocument.IssuedAt, pendingDocument.TypeCode);
                
                var request = GetDocumentRequestFromDocumentDTO(DocumentDTOCleanser(pendingDocument));
                Console.WriteLine("[DEBUG] JSON Request:");
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(request, Newtonsoft.Json.Formatting.Indented));
                var response = API.PostDocument(request);
               
                db.UpdateDocumentTribunetControlWithStatus(request.ControlNumber, (int)response.StatusCode);
                
                JObject jobject = JObject.Parse(response.Content);
                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    string documentKey = jobject["clave"].ToString();
                    string documentNumber = jobject["numero"].ToString();
                    
                    db.UpdateDocumentTribunetControlWithKeyAndNumber(request.ControlNumber, documentKey, documentNumber);
                    success.AppendFormat("* Documento [{0}] posteado y actualizado con la clave [{1}]", pendingDocument.ControlNumber, documentKey).AppendLine();
                }
                else
                {
                    string message = jobject["message"].ToString();
                    JArray jarray = (JArray)jobject["errors"];

                    error.AppendFormat("* Documento [{0}] posteado retorna el siguiente mensaje: [{1}]", request.ControlNumber, message);
                    if (jarray != null && jarray.Count > 0)
                    {
                        error.AppendFormat(", y los siguientes errores:").AppendLine();
                        for (int index = 0; index < jarray.Count; ++index)
                        {
                            error.AppendFormat("** [{0}] - [{1}]", jarray[index]["property"], jarray[index]["message"]).AppendLine();
                        }
                    }
                    else
                    {
                        error.AppendLine();
                    }
                }
            }

            if (success.Length > 0)
            {
                success.Insert(0, "Se listan los pendientes posteados exitosamente:\n");
                
                WriteEventLogEntry(success.ToString());
            }

            if (error.Length > 0)
            {
                error.Insert(0, "Se listan los pendientes posteados con errores:\n");

                WriteEventLogEntry(error.ToString());
            }
        }

        private static DocumentDTO DocumentDTOCleanser(DocumentDTO obj)
        {
            //obj.Items.ForEach(item =>
            //{
            //    item.Position = 1;
            //    item.UnitPrice = Math.Round(item.ItemTotalAmount / 1.13m, 2);
            //    item.TaxRate = 13;
            //    item.TaxAmount = item.ItemTotalAmount - item.UnitPrice;
            //    item.SubTotal = item.TotalAmount = item.UnitPrice;
            //});

            obj.TotalExemptedServices = obj.Items.Where(item => item.MeasurementUnitCode == "Sp" && item.TaxRate == 0).Sum(item => item.TotalAmount);
            obj.TotalTaxedServices = obj.Items.Where(item => item.MeasurementUnitCode == "Sp" && item.TaxRate > 0).Sum(item => item.TotalAmount);
            obj.TotalExempted = obj.TotalExemptedGoods + obj.TotalExemptedServices;
            obj.TotalTaxed = obj.TotalTaxedGoods + obj.TotalTaxedServices;
            obj.TotalSale = obj.TotalTaxed + obj.TotalExempted;
            obj.TotalNetSale = obj.TotalSale;
            obj.TotalTax = obj.Items.Sum(item => item.TaxAmount);
            obj.Total = obj.TotalNetSale + obj.TotalTax;
            
            // Agregar TaxBreakdown obligatorio
            obj.TaxBreakdown = new List<TaxBreakdownDTO>
            {
                new TaxBreakdownDTO
                {
                    TypeCode = "01", // IVA
                    VatTaxRateCode = "08", // 13%
                    Amount = obj.TotalTax
                }
            };
            
            return obj;
        }

        private static DocumentRequest GetDocumentRequestFromDocumentDTO(DocumentDTO source)
        {
            StringBuilder sbRemarks = new StringBuilder();
            if (!string.IsNullOrEmpty(source.ExtraLicensePlate))
            {
                sbRemarks.Append("PLACA: " + source.ExtraLicensePlate.ToUpper().Trim() + "\\n");
            }
            if (!string.IsNullOrEmpty(source.Remarks))
            {
                sbRemarks.Append(source.Remarks.ToUpper().Trim());
            }
            return new DocumentRequest
            {
                Header = new DocumentHeaderRequest
                {
                    BranchCode = source.BranchCode,
                    BranchTerminalCode = source.BranchTerminalCode,
                    DocumentKind = 1,
                    SecurityCode = DateTime.Now.ToString("yyyyMMdd"),
                    EconomicActivityCode = source.EconomicActivityCode
                },
                ControlNumber = source.ControlNumber.ToString(),
                Recipient = GetDocumentRecipientRequestFromDocumentDTO(source),
                IssuedAt = source.IssuedAt,
                TypeCode = source.TypeCode,
                SaleConditionCode = source.SaleConditionCode,
                //CreditTerm = source.CreditTerm,
				CreditTermInDays = source.CreditTermInDays,
                //PaymentMethodCodes = new List<String> { "01" },
                Items = source.Items.Select(GetDocumentItemRequestFromDocumentItemDTO).ToList(),
                Remarks = source.Remarks,
                PaymentsInformation = new DocumentPaymentInformationRequest[1]{ new DocumentPaymentInformationRequest
                    {
                        TypeCode = "01",
                        Amount = source.Total
                    }
                }.ToList(),
                Summary = new DocumentSummaryRequest
                {
                    CurrencyCode = source.CurrencyCode,
                    TotalTaxedGoods = source.TotalTaxedGoods,
                    TotalTaxedServices = source.TotalTaxedServices,
                    TotalTaxed = source.TotalTaxed,
                    TotalExemptedGoods = source.TotalExemptedGoods,
                    TotalExemptedServices = source.TotalExemptedServices,
                    TotalExempted = source.TotalExempted,
                    TotalSale = source.TotalSale,
                    TotalDiscounts = source.TotalDiscounts,
                    TotalNetSale = source.TotalNetSale,
                    TotalTax = source.TotalTax,
                    Total = source.Total,
                    TotalUntaxedServices = source.TotalUntaxedServices,
                    TotalUntaxedGoods = source.TotalUntaxedGoods,
                    TotalUntaxed = source.TotalUntaxed,
                    TotalTaxAssumedIssuerFactory = source.TotalTaxAssumedIssuerFactory,
                    TaxBreakdown = source.TaxBreakdown?.Select(tb => new DocumentSummaryTaxBreakdownRequest
                    {
                        TypeCode = tb.TypeCode,
                        VatTaxRateCode = tb.VatTaxRateCode,
                        Amount = tb.Amount
                    }).ToList()
                }
            };
        }

        private static DocumentItemRequest GetDocumentItemRequestFromDocumentItemDTO(DocumentItemDTO source)
        {
            if (source == null)
            {
                return null;
            }
            DocumentItemRequest target = new DocumentItemRequest();
            target.Position = source.Position;
            target.Codes = new List<DocumentItemCodeRequest>
            {
                new DocumentItemCodeRequest
                {
                  Code = source.ItemCode,
                  Type = "01"
                }
            };
            target.Quantity = source.Quantity;
            target.CabysCode = "8714100009900";
            target.MeasurementUnitCode = source.MeasurementUnitCode;
            target.Description = source.Description;
            target.UnitPrice = source.UnitPrice;
            target.TotalAmount = source.TotalAmount;
            target.SubTotal = source.SubTotal;
			target.TaxableBase = source.TaxableBase;
            target.ItemTotalAmount = source.ItemTotalAmount;
            target.NetTaxAmount = source.NetTaxAmount;
            target.Taxes = new List<DocumentItemTaxRequest>
            {
                new DocumentItemTaxRequest
                {
                    Amount = source.TaxAmount,
                    Rate = source.TaxRate,
                    TypeCode = "01",
                    VatTaxRateCode = "08"
                }
            };
            return target;
        }

        private static DocumentRecipientDocumentRequest GetDocumentRecipientDocumentRequestFromRecipientDocumentNumber(string source)
        {
            if (source == null)
            {
                return null;
            }

            string str = new string(source.Where(char.IsDigit).ToArray()).TrimStart('0');
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            DocumentRecipientDocumentRequest target = new DocumentRecipientDocumentRequest
            {
                Number = str
            };

            if (str.Length == 9)
            {
                target.TypeCode = "01";
            }
            else if (str.Length == 10)
            {
                target.TypeCode = str.StartsWith("312") ? "04" : "02";
            }
            else
            {
                if (str.Length != 11 && str.Length != 12)
                {
                    return null;
                }
                target.TypeCode = "03";
            }
            return target;
        }

        private static DocumentRecipientRequest GetDocumentRecipientRequestFromDocumentDTO(DocumentDTO source)
        {
            if (string.IsNullOrEmpty(source.RecipientName))
            {
                return null;
            }

            DocumentRecipientRequest target = new DocumentRecipientRequest()
            {
                Name = source.RecipientName.Trim(),
                Document = GetDocumentRecipientDocumentRequestFromRecipientDocumentNumber(source.RecipientDocumentNumber)
            };

            if (!string.IsNullOrEmpty(source.RecipientEmailAddress))
            {
                int length = source.RecipientEmailAddress.IndexOf(",");
                if (length >= 0)
                {
                    source.RecipientEmailAddress = source.RecipientEmailAddress.Substring(0, length);
                }

                if (IsValidEmailAddress(source.RecipientEmailAddress))
                {
                    target.EmailAddress = source.RecipientEmailAddress.Trim();
                }
            }
            return target;
        }

        private static bool IsValidEmailAddress(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                return false;
            }

            return new Regex("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$").Match(emailAddress).Success;
        }
    }
}
