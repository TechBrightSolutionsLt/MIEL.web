using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MIEL.web.Models.EntityModels;

namespace MIEL.web.Models.ViewModels
{
    public class CategoryMasterVM
    {
        public List<Category> Categories { get; set; } = new List<Category>();

        public Category Category { get; set; } = new Category();
        [ValidateNever]
        public IEnumerable<SelectListItem> MainCategoryList { get; set; }
    }
}
