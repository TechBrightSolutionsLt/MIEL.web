using Microsoft.AspNetCore.Mvc.Rendering;
using MIEL.web.Models.EntityModels;

namespace MIEL.web.Models.ViewModel
{
    public class CategorySpecificationVM
    {
        public int Id { get; set; }
        public string SpecName { get; set; } = string.Empty;
        public int SelectedCategoryId { get; set; }
        public string Options { get; set; }
        public string? OptionType { get; set; }

        // Uses your existing Category entity
        public List<Category> CategoryList { get; set; } = new List<Category>();

        // For dropdown binding
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}
