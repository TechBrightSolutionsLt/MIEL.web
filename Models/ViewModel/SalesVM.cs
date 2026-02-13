using Microsoft.AspNetCore.Mvc.Rendering;

namespace MIEL.web.Models.ViewModel
{
    public class SalesVM
    {
        // ===== MASTER =====
        public int SalesId { get; set; }

        public string InvoiceNo { get; set; }

        public DateTime SalesDate { get; set; } = DateTime.Now;
        public int CustomerId { get; set; }

        public string PaymentType { get; set; }   // Cash / Card / UPI / Bank

        // ===== TOTALS =====
        public decimal TotalAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal GstAmount { get; set; }
        public decimal NetAmount { get; set; }
       

        // ===== DROPDOWNS =====
        public List<SelectListItem> PaymentTypes { get; set; } = new()
        {
            new SelectListItem { Text = "Cash", Value = "Cash" },
            new SelectListItem { Text = "Card", Value = "Card" },
            new SelectListItem { Text = "UPI", Value = "UPI" },
            new SelectListItem { Text = "Bank", Value = "Bank" }
        };

        // ===== ITEMS =====
        public List<SalesItemVM> Items { get; set; } = new();
    }
}
