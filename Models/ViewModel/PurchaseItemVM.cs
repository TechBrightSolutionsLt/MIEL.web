namespace MIEL.web.Models.ViewModel
{
    public class PurchaseItemVM
    {
        public string ProductName { get; set; }
        public string VariantCode { get; set; }
        public decimal Rate { get; set; }
        public int Quantity { get; set; }
        public decimal DiscPercent { get; set; }
        public decimal DiscAmount { get; set; }
        public decimal GstPercent { get; set; }
        public decimal GstAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal SellingPrice { get; set; }
    }
}
