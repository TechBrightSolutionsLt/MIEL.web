using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIEL.web.Models.EntityModels
{
    public class procolrsizevarnt
    {
        [Key]
        public int varientid { get; set; }

        [ForeignKey("ProductMaster")]
        public int ProductId { get; set; }
        
       
    public string colour { get; set; }
    public string size { get; set; }
        public int QuantityOnHand { get; set; }
        public string varientCode { get; set; }
        public decimal AverageCost { get; set; }

    }
}
