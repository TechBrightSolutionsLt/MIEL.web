using MIEL.web.Models.EntityModels;

namespace MIEL.web.Models.ViewModel
{
    public class ReviewOrderVM
    {
        public List<CartItem> CartItems { get; set; }
        public Customer Address { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
