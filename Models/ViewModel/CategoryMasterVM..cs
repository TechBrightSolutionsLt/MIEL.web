using System.Collections.Generic;
using MIEL.web.Models.EntityModels;

namespace MIEL.web.Models.ViewModels
{
    public class CategoryMasterVM
    {
        public List<Category> Categories { get; set; } = new List<Category>();

        public Category Category { get; set; } = new Category();
    }
}
