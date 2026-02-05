using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.ViewModel
{
    public class UserProfileVM
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [Required, MaxLength(100)]
        public string LastName { get; set; }

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; }

        [Required, MaxLength(15)]
        public string MobileNumber { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string Address { get; set; }

        [Required, MaxLength(100)]
        public string City { get; set; }

        [Required, MaxLength(10)]
        public string Postcode { get; set; }
    }
}
