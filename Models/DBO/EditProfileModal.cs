using System.ComponentModel.DataAnnotations;

namespace Assignment.Models.DBO
{
    public class EditProfileModal
    {
        [Required]
        public string FullName { set; get; }

        [Required]
        [EmailAddress]
        public string Email { set; get; }
        [Required]
        public string? PhoneNumber { set; get; }

        [Required(ErrorMessage = "Location cannot be empty !!")]
        public string Address { set; get; }

        public IFormFile? ProfilePicture { get; set; }

        public string? ProfilePictureUrl { get; set; }
    }
}
