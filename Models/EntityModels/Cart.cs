namespace MIEL.web.Models.EntityModels
{
    public class Cart
    {
        public int CartId { get; set; }

        public int CustomerId { get; set; }

        public int ProductId { get; set; }

        public int VariantId { get; set; }

        public string ProductName { get; set; }

        public string Color { get; set; }

        public string Size { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string Image { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
