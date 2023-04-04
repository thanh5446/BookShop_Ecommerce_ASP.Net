using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

namespace Assignment.Models
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { set; get; }
        [Required]
        public int OrderID { get; set; }
        [Required]
        public int BookID { get; set; }

        [Required]
        public int Quantity { get; set; }
        [Required]
        [Column(TypeName = "Money")]
        public decimal Total { get; set; }
        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }

        [ForeignKey("ProductID")]
        public virtual Book Book { get; set; }
    }
}
