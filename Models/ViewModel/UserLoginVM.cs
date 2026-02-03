using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.ViewModel
{
    public class UserLoginVM
    {
        [Required(ErrorMessage = "Email or Mobile number is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
