using Microsoft.AspNetCore.Mvc.Rendering;

namespace MIEL.web.Models.ViewModel
{
    public class CategorySpecificationVM
    {
        public int Id { get; set; }
        public int SpecName { get; set; }
        public int SelectedCategoryId { get; set; }

        //Uses your existing Category entity
        public List<Categories> CategoryList { get; set; } = new List<Categories>();

        // For dropdown binding
        public List<SelectListItem> Categories { get; set; }  = new List<SelectListItem>();
    }
}
