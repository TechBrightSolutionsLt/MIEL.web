using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIEL.web.Models.EntityModels
{
    public class productspecification
    {

        [Key]
        public int sId { get; set; }

        [ForeignKey("ProductMaster")]
        public int ProductId { get; set; }

        //[ForeignKey("Specifications")]
        [ForeignKey("CategorySpecification")]
        public int Id { get; set; }
        public string specificationvalue { get; set; }

        public virtual CategorySpecification CategorySpecification { get; set; }

    }
}
