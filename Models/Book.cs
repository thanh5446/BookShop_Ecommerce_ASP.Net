using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment.Models
{
    [Table("Book")]
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { set; get; }

        [Required(ErrorMessage = "Name cannot be empty !!")]
        [StringLength(50)]
        public string Name { set; get; }

        [Column(TypeName = "Money")]
        [Required(ErrorMessage = "Price cannot be empty !!")]
        public decimal Price { set; get; }
        [Column(TypeName = "Money")]
        public decimal DiscountPrice { set; get; }

       [StringLength(500)]
        [Required(ErrorMessage = "Description cannot be empty !!")]
        public string Description { set; get; }

        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
        [Required]
        public int Quantity { set; get; }

        [Required(ErrorMessage = "Image cannot be empty !!")]
        public string Image { set; get; }

        [StringLength(50)]
        [Required(ErrorMessage = "Author cannot be empty !!")]
        public string Author { set; get; }

        [StringLength(50)]
        [Required(ErrorMessage = "Status cannot be empty !!")]
        public string Status { set; get; }

        [Required]
        public int CategoryID { set; get; }

        [ForeignKey("CategoryID")]
        public virtual Category? Category { get; set; }
    }
}
