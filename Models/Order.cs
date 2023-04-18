using Assignment.DataAccess;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment.Models
{
    [Table("Order")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { set; get; }
        [Required]
        public string UserID { set; get; }

        [Column(TypeName = "Money")]
        public decimal? Total { set; get; }

        [Required]
        public DateTime OrderDate { set; get; }

        [Required]
        [StringLength(50)]
        public string Status { set; get; }

        [StringLength(50)]
        public string PaymentMethod { set; get; }

        public bool Delete { get; set; }

        [StringLength(250)]
        public string? Note { set; get; }

        [ForeignKey("UserID")]
        public virtual AppUser? User { get; set; }
        public virtual IEnumerable<OrderDetail>? OrderDetails { set; get; } = new List<OrderDetail>();
    }
}
