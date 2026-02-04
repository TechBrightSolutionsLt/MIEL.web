using MIEL.web.Models.EntityModels;
//using YourNamespace.Models;

namespace MIEL.web.Models.ViewModel
{
    public class ProductCreateViewModel
    {
        public Product Product { get; set; }

        public List<Category> Categories { get; set; }

        public List<CategorySpecification> Specifications { get; set; }

        public List<string> SelectedSizes { get; set; } = new();
    }
}
