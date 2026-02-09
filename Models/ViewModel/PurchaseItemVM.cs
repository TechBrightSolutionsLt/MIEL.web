namespace MIEL.web.Models.ViewModel
{
    public class PurchaseItemVM
    {
        public string ProductName { get; set; }
        public string VariantCode { get; set; }
        public decimal Rate { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }
}
