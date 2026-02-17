namespace MIEL.web.Models.ViewModel
{
    public class PurchaseListVM
    {
        public int PurchaseId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string SupplierName { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
