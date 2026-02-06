
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIEL.web.Models.EntityModels
{
    public class ProductImages
    {
        [Key]
        public int ImgId { get; set; }

        [Required]
        [ForeignKey("ProductMaster")]
        public int ProductId { get; set; }
        public ProductMaster ProductMaster { get; set; }



        public string ImgPath { get; set; }
        public int Flag { get; set; }   // 1 = main image, 0 = others

    }
}
