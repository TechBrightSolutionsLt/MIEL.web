
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIEL.web.Models.EntityModels
{
    public class ProductImage
    {
        [Key]
        public int ImgId { get; set; }

        [ForeignKey("ProductMaster")]
        public int ProductId { get; set; }
        public ProductMaster ProductMaster { get; set; }

        [Required]
        public string ImgPath { get; set; }
        public int Flag { get; set; }   // 1 = main image, 0 = others

    }
}
