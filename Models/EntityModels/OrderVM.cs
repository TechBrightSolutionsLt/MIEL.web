using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.EntityModels
{
    public class OrderVM
    {

        public int Id { get; set; }
        public int CustomerId { get; set; }


        public string OrderNumber { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentStatus { get; set; }  // 0=Pending,1=Paid,2=COD

        public string PayId { get; set; }  // Transaction ID / Payment ID

        public int? VerifyId { get; set; }  // Admin ID

        public string BankReference { get; set; }

        public DateTime? VerifiedDate { get; set; }
    }
}
