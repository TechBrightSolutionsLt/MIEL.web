using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIEL.web.Models.EntityModels
{
    public class SalesItem
    {
        [Key]
        public int SalesItemId { get; set; }

        public int SalesId { get; set; }

        [ForeignKey("SalesId")]
        public SalesMaster SalesMaster { get; set; }

        // 🔥 FOREIGN KEY TO VARIANT
        public int varientid { get; set; }

        [ForeignKey("varientid")]
        public procolrsizevarnt procolrsizevarnt { get; set; }

        [MaxLength(50)]
        public string BatchNo { get; set; }

        public int Quantity { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal DiscPercent { get; set; }
        public decimal DiscAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal NetAmount { get; set; }
    }
}