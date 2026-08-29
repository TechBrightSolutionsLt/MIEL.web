using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.EntityModels
{
    public class OrderVM
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ? SalesId { get; set; }
        public string OrderNumber { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentStatus { get; set; }

        public string? PayId { get; set; }

        public int? VerifyId { get; set; }

        public string? BankReference { get; set; }

        public DateTime? VerifiedDate { get; set; }
        public userModel Customer { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public DateTime? OrderDate { get; set; }

    }

}
