namespace MIEL.web.Models.ViewModel
{
    public class SalesItemVM
    {
        public int varientid { get; set; }

        public string ProductName { get; set; }
        public string VariantCode { get; set; }

        public string BatchNo { get; set; }

        public int Quantity { get; set; }

        // ===== PRICING =====
        public decimal SellingPrice { get; set; } // GST INCLUDED
        public decimal DiscPercent { get; set; }
        public decimal DiscAmount { get; set; }

        public decimal TaxAmount { get; set; }     // 10% INCLUDED GST
        public decimal NetAmount { get; set; }
    }
}
