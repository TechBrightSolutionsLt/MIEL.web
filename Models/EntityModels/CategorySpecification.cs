using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.EntityModels
{
    public class CategorySpecification
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string SpecName { get; set; }
        public int CategoryId { get; set; }
        public string Options { get; set; }
    }
}
