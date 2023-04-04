using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { set; get; }

        [Required(ErrorMessage = "Name cannot be empty !!")]
        [StringLength(50)]
        public string FullName { set; get; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                            ErrorMessage = "Please enter a valid email address")]
        public string Email { set; get; }

        [Required(ErrorMessage = "Password cannot be empty !!")]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string Password { set; get; }

        [Required(ErrorMessage = "Confirm password cannot be empty !!")]
        [StringLength(50)]
        [DataType(DataType.Password)]
        [NotMapped]
        public string ConfirmPassword { set; get; }

        [StringLength(20)]
        public string PhoneNumber { set; get; }

        [Required(ErrorMessage = "Location cannot be empty !!")]
        [StringLength(50)]
        public string Location { set; get; }

        [Required(ErrorMessage = "Role cannot be empty !!")]
        [StringLength(50)]
        public string Role { set; get; }

        public string? Avatar { set; get; }
    }
}
