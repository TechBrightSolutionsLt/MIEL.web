using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public int paysts { get; set; }
        public int salesmode { get; set; }

        // ================= FOREIGN KEY =================

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        // ✅ ADD THIS (VERY IMPORTANT)
        public userModel Customer { get; set; }


        // ================= NAVIGATION =================

        public ICollection<SalesItem> SalesItems { get; set; } = new List<SalesItem>();
    }
}
