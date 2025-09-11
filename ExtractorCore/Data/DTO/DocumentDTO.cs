using System;
using System.Collections.Generic;

namespace ExtractorFacturero.Core.Data.DTO
{
    public class DocumentDTO
    {
        public string BranchCode { get; set; }

        public string BranchTerminalCode { get; set; }

        public string SecurityCode { get; set; }

        public string ControlNumber { get; set; }

        public DateTime IssuedAt { get; set; }

        public string IssuedAtDate { get; set; }

        public string IssuedAtTime { get; set; }

        public string TypeCode { get; set; }

        public string SaleConditionCode { get; set; }

        //public string CreditTerm { get; set; }

        public string CreditTermInDays { get; set; }

        public string PaymentMethodCode { get; set; }

        public string RecipientName { get; set; }

        public string RecipientDocumentType { get; set; }

        public string RecipientDocumentNumber { get; set; }

        public string RecipientPhoneNumber { get; set; }

        public string RecipientEmailAddress { get; set; }

        public string CurrencyCode { get; set; }

        public Decimal TotalExemptedGoods { get; set; }

        public Decimal TotalExemptedServices { get; set; }

        public Decimal TotalExempted { get; set; }

        public Decimal TotalSale { get; set; }

        public Decimal TotalDiscounts { get; set; }

        public Decimal TotalTaxedGoods { get; set; }

        public Decimal TotalTaxedServices { get; set; }

        public Decimal TotalTaxed { get; set; }

        public Decimal TotalTax { get; set; }

        public Decimal TotalNetSale { get; set; }

        public Decimal Total { get; set; }

        public DateTime? ReferenceIssuedAt { get; set; }

        public string ReferenceControlNumber { get; set; }

        public string ReferenceKey { get; set; }

        public string ReferenceTypeCode { get; set; }

        public string Remarks { get; set; }

        public string ExtraLicensePlate { get; set; }

        public List<DocumentItemDTO> Items { get; set; }
        public string EconomicActivityCode { get; set; }
        public Decimal TotalUntaxedServices { get; set; }
        public Decimal TotalUntaxedGoods { get; set; }
        public Decimal TotalUntaxed { get; set; }
        public Decimal TotalTaxAssumedIssuerFactory { get; set; }
        public List<TaxBreakdownDTO> TaxBreakdown { get; set; }
    }
}
