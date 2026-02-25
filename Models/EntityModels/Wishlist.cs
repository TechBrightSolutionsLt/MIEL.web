using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.EntityModels
{
    public class Wishlist
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int? CustomerId { get; set; }   // For logged users

        public string? GuestId { get; set; }    // For guest users

       
        public string? ProductName { get; set; }

        public decimal Price { get; set; }


        public string? Image { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
