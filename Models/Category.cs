using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { set; get; }

        [Required(ErrorMessage = "Name cannot be empty !!")]
        [StringLength(50)]
        public string Name { set; get; }

        [StringLength(50)]
        public string? Alias { set; get; }

        [Required(ErrorMessage = "Status cannot be empty !!")]
        [StringLength(50)]
        public string Status { set; get; } = "Processing";

        [StringLength(250)]
        public string? Description { set; get; }

        public virtual List<Book>? Books { get; set; } = new List<Book>();
    }
}
