using MIEL.web.Models.EntityModels;

namespace MIEL.web.Models.ViewModel
{
    public class ProductEditVM
    {
        public ProductMaster Product { get; set; }
        public List<Category> Categories { get; set; }
        public List<ProductImages> Images { get; set; }
        public List<procolrsizevarnt> Variants { get; set; }
        public List<productspecification> Specifications { get; set; }
    }
}
