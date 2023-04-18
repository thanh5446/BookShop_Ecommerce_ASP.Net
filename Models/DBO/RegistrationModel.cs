using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment.Models.DBO
{
    public class RegistrationModel
    {
        [Required]
        public string FullName { set; get; }

        [Required]
        [EmailAddress]
        public string Email { set; get; }

        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[#$^+=!*()@%&]).{6,}$", ErrorMessage = "Minimum length 6 and must contain  1 Uppercase,1 lowercase, 1 special character and 1 digit")]
        public string Password { set; get; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { set; get; }
        [Required]
        public string? PhoneNumber { set; get; }

        [Required(ErrorMessage = "Location cannot be empty !!")]
        public string Address { set; get; }
        public string? Role { set; get; }
    }
}
