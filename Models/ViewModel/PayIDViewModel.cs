namespace MIEL.web.Models.ViewModel
{
    public class PayIDViewModel
    {
        public int SalesId { get; set; }
        public string InvoiceNo { get; set; }
        public decimal TotalAmount { get; set; }
        public int OrderId { get; set; }        // Newly added
        public string OrderNumber { get; set; }
        public string PaymentStatus { get; set; }
        public string PayId { get; set; }
        //   public string Email { get; set; }
        public string CustomerName { get; set; }
        public string BusinessEmail { get; set; }   // shown on page
        public string CustomerEmail { get; set; }   // used for EmailJS
        public string ItemsSummary { get; set; }
    }
}
