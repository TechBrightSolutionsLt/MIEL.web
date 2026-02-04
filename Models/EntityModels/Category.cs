using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.EntityModels
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(100)]
        public string CategoryName { get; set; }
    }
}
