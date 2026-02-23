using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.ViewModel
{
    public class VerifyPaymentVM
    {
        public int Id { get; set; }

        public string? Email { get; set; }
        public string? OrderNumber { get; set; }
        public string ?FirstName { get; set; }
        public decimal TotalAmount { get; set; }

        public string? PaymentType { get; set; }

      
        public string? BankReference { get; set; }
    }
}
