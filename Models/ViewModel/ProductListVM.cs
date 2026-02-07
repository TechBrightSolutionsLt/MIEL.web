namespace MIEL.web.Models.ViewModel
{
    public class ProductListVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string ImagePath { get; set; }


        public string ProductDescription { get; set; }
        public List<string> Images { get; set; }


    }
}
