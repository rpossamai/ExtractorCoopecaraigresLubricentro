using System;
namespace ExtractorFacturero.Core.Data.DTO
{
    public class TaxBreakdownDTO
    {
        public string TypeCode { get; set; }
        public string VatTaxRateCode { get; set; }
        public Decimal Amount { get; set; }
    }
}
