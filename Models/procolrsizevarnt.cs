using System.ComponentModel.DataAnnotations.Schema;

namespace MIEL.web.Models
{
    public class procolrsizevarnt
    {
        [ForeignKey("ProductMaster")]
        public int ProductId { get; set; }
        public int varientid { get; set; }
    public string colour { get; set; }
    public string size { get; set; }

    }
}
