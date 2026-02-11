namespace MIEL.web.Models.ViewModel
{
    public class ColorSizeVM
    {
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Rate { get; set; }
        public int VariantId { get; set; }
    }
    public class SpecificationVM
    {
        public string SpecName { get; set; }
        public string SpecValue { get; set; }
    }
    public class ProductListVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string ImagePath { get; set; }

        public decimal? NetAmount { get; set; }

        public string ProductDescription { get; set; }
        public List<string> Images { get; set; }
        //public List<string> Specifications { get; set; }

        public List<ColorSizeVM> Variants { get; set; }
        public List<SpecificationVM> Specificationsnew{ get; set; }

    }

}
