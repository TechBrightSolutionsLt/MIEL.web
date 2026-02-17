using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIEL.web.Models.EntityModels
{
    public class SalesItem
    {
        [Key]
        public int SalesItemId { get; set; }

        public int SalesId { get; set; }
        [ForeignKey("SalesId")]   // ✅ Tell EF this is the FK for SalesMaster
        public SalesMaster SalesMaster { get; set; }
        public int varientid { get; set; }

        [MaxLength(50)]
        public string BatchNo { get; set; }

        public int Quantity { get; set; }


        // GST INCLUDED price (AUD)
        public decimal SellingPrice { get; set; }

        // ================= DISCOUNT =================
        public decimal DiscPercent { get; set; }     // eg: 10%
        public decimal DiscAmount { get; set; }      // calculated

        // ================= TAX =================
        public decimal TaxAmount { get; set; }       // GST portion (10% INCLUDED)

        // ================= TOTAL =================
        public decimal NetAmount { get; set; }       // final payable
    }
}
