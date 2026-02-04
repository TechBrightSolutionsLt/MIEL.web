using Microsoft.AspNetCore.Mvc.Rendering;
using YourNamespace.Models;

namespace MIEL.web.Models.ViewModel
{
    public class CategorySpecificationVM
    {
        public int Id { get; set; }
        public string SpecName { get; set; }
        public int SelectedCategoryId { get; set; }

        //Uses your existing Category entity
        public List<Category> CategoryList { get; set; } = new List<Category>();

        // For dropdown binding
        public List<SelectListItem> Categories { get; set; }  = new List<SelectListItem>();
    }
}
