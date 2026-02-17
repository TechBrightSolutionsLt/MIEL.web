using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIEL.web.Models.EntityModels
{
    public class SalesItem
    {
        [Key]
        public int SalesItemId { get; set; }

        // 🔥 Foreign Key
        public int SalesId { get; set; }

        [ForeignKey("SalesId")]
        public SalesMaster SalesMaster { get; set; }

        public int varientid { get; set; }

        [MaxLength(50)]
        public string BatchNo { get; set; }

        public int Quantity { get; set; }

        // GST INCLUDED price (AUD)
        public decimal SellingPrice { get; set; }

        // ================= DISCOUNT =================
        public decimal DiscPercent { get; set; }
        public decimal DiscAmount { get; set; }

        // ================= TAX =================
        public decimal TaxAmount { get; set; }

        // ================= TOTAL =================
        public decimal NetAmount { get; set; }
    }
}
