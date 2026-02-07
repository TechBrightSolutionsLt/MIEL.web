using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.EntityModels
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        
        [Required(ErrorMessage = "Supplier Name is required")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only alphabets are allowed in Name")]
        public string Name { get; set; }
        
        [Required]
        public string Type { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^[0-9]{10,}$", ErrorMessage = "Please enter at least 10 digits")]
        public string Mobile { get; set; }


        [Required, EmailAddress(ErrorMessage = "Invalid email address")]
        public string EmailId { get; set; }

        [Required]
        public string Street { get; set; }
        [Required]
        public string BuildingName { get; set; }
        [Required]
        public string BuildingNo { get; set; }
        [Required]
        public string Landmark { get; set; }
        [Required]
        public string Coordinates { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required(ErrorMessage = "Postal Code is required")]
        [StringLength(10)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Postal Code must contain only numbers")]
        public string Pin { get; set; }
        [Required(ErrorMessage = "GST Number is required")]
        [StringLength(15, MinimumLength = 15, ErrorMessage = "GST Number must be 15 characters")]
        [RegularExpression(
            @"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$",
            ErrorMessage = "Invalid GST Number format"
        )]
        public string GstNo { get; set; }

        [Required(ErrorMessage = "Credit Limit is required")]
        [Range(0.01, 999999999, ErrorMessage = "Credit Limit must be greater than 0")]
       
        public decimal? CreditLimit { get; set; }
    }
}
