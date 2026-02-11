using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.EntityModels
{
    public class InventoryBatch
    {
        [Key]
        public int InventoryBatchId { get; set; }

        public int varientid { get; set; }

        [Required]
        [MaxLength(20)]
      
        public string BatchNo { get; set; }

        public int QuantityIn { get; set; }
        public int QuantityOut { get; set; }

        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }

        public DateTime CreatedDate { get; set; }
       
    }
}
