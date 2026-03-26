namespace MIEL.web.Models.ViewModel
{
    public class PurchaseSummaryVM
    {
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal GstAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
