using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.EntityModels
{
    public class VariantPrice
    {
        [Key]
        public int VariantPriceId { get; set; }

        public int varientid { get; set; }

        public decimal SellingPrice { get; set; }

        public bool IsActive { get; set; }
    }
}
