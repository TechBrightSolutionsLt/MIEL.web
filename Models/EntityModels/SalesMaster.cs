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
