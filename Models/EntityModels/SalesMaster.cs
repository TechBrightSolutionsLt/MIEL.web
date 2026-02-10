using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.EntityModels
{
    public class SalesMaster
    {
        [Key]
        public int SalesId { get; set; }

        public DateTime SalesDate { get; set; } = DateTime.Now;

        [MaxLength(30)]
        public string InvoiceNo { get; set; }

        [MaxLength(20)]
        public string PaymentType { get; set; } // Cash / Card / UPI / Bank

        // ================= TOTALS =================

        public decimal TotalAmount { get; set; }      // Sum of item gross (before discount)
        public decimal TotalDiscount { get; set; }    // 🔴 REQUIRED
        public decimal GstAmount { get; set; }        // GST INCLUDED (10%)
        public decimal NetAmount { get; set; }        // Final payable

        // ================= NAV =================
        public ICollection<SalesItem> SalesItems { get; set; }
    }
}
