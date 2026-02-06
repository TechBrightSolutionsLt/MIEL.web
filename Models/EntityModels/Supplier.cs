using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.EntityModels
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Supplier Code is required"), StringLength(20)]
        public string Code { get; set; }

        [Required(ErrorMessage = "Supplier Name is required")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only alphabets are allowed in Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Supplier Type is required")]
        public string Type { get; set; } // Manufacturer / Trader / Service

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } // Active / Inactive

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Address is required"), StringLength(250)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        [StringLength(10)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Postal Code must contain only numbers")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^[0-9]{10,}$", ErrorMessage = "Please enter at least 10 digits")]
        public string Phone { get; set; }

        [Required, EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "GSTIN is required")]
        [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[A-Z0-9]{3}$",
 ErrorMessage = "Invalid GSTIN format")]
        public string GSTIN { get; set; }

        // Nullable (can store NULL)
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Bank Name must contain only letters")]
        public string? BankName { get; set; }
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Account Number must contain only numbers")]
        public string? AccountNumber { get; set; }
        [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC Code format")]
        public string? IFSC { get; set; }
    }
}
