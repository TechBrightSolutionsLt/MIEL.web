using System;
using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.EntityModels
{
    public class userModel
    {
        [Key]
        public int CustomerId { get; set; }
        [Required]
        public int RoleId { get; set; }
        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [Required, MaxLength(100)]
        public string LastName { get; set; }

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; }

        [Required, MaxLength(15)]
        public string MobileNumber { get; set; }

        [Required, MaxLength(10)]
        public string Gender { get; set; }

        [Required]
        public string Address { get; set; }

        [Required, MaxLength(100)]
        public string City { get; set; }

        [Required, MaxLength(10)]
        public string Postcode { get; set; }

        [Required]
        public string Password { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
