using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.EntityModels
{
    public class MainCategory
    {
        [Key]
        public int MainCategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(100)]
        public string MainCategoryName { get; set; }
    }
}
