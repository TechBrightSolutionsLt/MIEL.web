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
        public string PaymentType { get; set; }

        // ================= TOTALS =================
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }      // Sum of item gross (before discount)
        public decimal TotalDiscount { get; set; }    // 🔴 REQUIRED
        public decimal GstAmount { get; set; }        // GST INCLUDED (10%)
        public decimal NetAmount { get; set; }        // Final payable
        public int paysts { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal GstAmount { get; set; }
        public decimal NetAmount { get; set; }

        public int salesmode { get; set; }

        // 🔥 Foreign Key to Customer (users_TB)
        public int CustomerId { get; set; }

        // ================= NAVIGATION =================

        public ICollection<SalesItem> SalesItems { get; set; } = new List<SalesItem>();
    }
}
