using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.EntityModels
{
    public class PurchaseItem
    {
        [Key]
        public int PurchaseItemId { get; set; }
        public int PurchaseId { get; set; }

        public int varientid { get; set; }
        public int Quantity { get; set; }

        public decimal Rate { get; set; }
        public string BatchNo { get; set; }
        public decimal GstPercent { get; set; }
        public decimal GstAmount { get; set; }

        public decimal DiscPercent { get; set; }
        public decimal DiscAmount { get; set; }

        public decimal TaxableAmount { get; set; }
        public decimal NetAmount { get; set; }
    }
}
