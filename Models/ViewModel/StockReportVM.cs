namespace MIEL.web.Models.ViewModel
{
    public class StockReportVM
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // Batch Detail
        public List<BatchDetailVM> BatchDetails { get; set; }

        // Product Summary
        public List<ProductSummaryVM> ProductSummary { get; set; }

        // Variant Summary
        public List<VariantSummaryVM> VariantSummary { get; set; }
    }

    public class BatchDetailVM
    {
        public string ProductName { get; set; }
        public string Colour { get; set; }
        public string Size { get; set; }
        public string BatchNo { get; set; }

        public int QuantityIn { get; set; }
        public int QuantityOut { get; set; }
        public int CurrentStock { get; set; }

        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
    }

    public class ProductSummaryVM
    {
        public string ProductName { get; set; }
        public int TotalIn { get; set; }
        public int TotalOut { get; set; }
        public int CurrentStock { get; set; }
    }

    public class VariantSummaryVM
    {
        public string ProductName { get; set; }
        public string Colour { get; set; }
        public string Size { get; set; }

        public int CurrentStock { get; set; }
    }
}
