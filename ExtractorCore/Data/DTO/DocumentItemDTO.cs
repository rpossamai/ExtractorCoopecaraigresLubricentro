using System;

namespace ExtractorFacturero.Core.Data.DTO
{
    public class DocumentItemDTO
    {
        public int Position { get; set; }

        public string ItemCode { get; set; }

        public string CabysCode { get; set; }

        public string Description { get; set; }

        public Decimal Quantity { get; set; }

        public string MeasurementUnitCode { get; set; }

        public Decimal UnitPrice { get; set; }

        public Decimal TotalAmount { get; set; }

        public Decimal TaxRate { get; set; }

        public Decimal TaxAmount { get; set; }

        public Decimal DiscountRate { get; set; }

        public Decimal DiscountAmount { get; set; }

        public string DiscountDescription { get; set; }

        public Decimal SubTotal { get; set; }

        public Decimal ItemTotalAmount { get; set; }
        public Decimal TaxAssumedIssuerFactory { get; set; }
        public Decimal? TaxableBase { get; set; }
        public string VinOrSerialNumber { get; set; }
        public string TransactionTypeCode { get; set; }
        public string FactoryVatCode { get; set; }
        public string MedicineRegistration { get; set; }
        public string PharmaceuticalFormCode { get; set; }
        public DiscountDTO Discount { get; set; }
        public Decimal NetTaxAmount { get; set; }
    }
}
