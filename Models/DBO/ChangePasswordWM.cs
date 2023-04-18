using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment.Models.DBO
{
    public class ChangePasswordWM
    {
        [Required(ErrorMessage = "Password cannot be empty !!")]
        [DataType(DataType.Password)]
        public string? Password { set; get; }

        [Required(ErrorMessage = "Confirm password cannot be empty !!")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string? ConfirmPassword { set; get; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Code { get; set; }
    }
}
