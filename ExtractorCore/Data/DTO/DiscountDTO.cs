using System;

namespace ExtractorFacturero.Core.Data.DTO
{
    public class DiscountDTO
    {
        public string TypeCode { get; set; }
        public Decimal Amount { get; set; }
        public string Description { get; set; }
    }
}