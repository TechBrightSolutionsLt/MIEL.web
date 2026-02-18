using MIEL.web.Models.EntityModels;

namespace MIEL.web.Models.ViewModel
{
    public class PaymentVM
    {
        public int SalesId { get; set; }
        public string InvoiceNo { get; set; }
        public decimal TotalAmount { get; set; }
        public int PayStatus { get; set; }
      public userModel Address { get; set; }
        public List<CartItem> Items { get; set; }
    }
}
