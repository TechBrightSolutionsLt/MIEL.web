using YourNamespace.Models;

namespace MIEL.web.Models.EntityModels
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string StitchingType { get; set; }
        public string Occasion { get; set; }
        public string PackageDetails { get; set; }

        // ☑ Sizes saved here (ex: "S,M,XL")
        public string AvailableSize { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
