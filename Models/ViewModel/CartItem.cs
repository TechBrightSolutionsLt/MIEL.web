namespace MIEL.web.Models.ViewModel
{
    public class CartItem
    {



        public int VariantId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int Change { get; set; }
    }
}
