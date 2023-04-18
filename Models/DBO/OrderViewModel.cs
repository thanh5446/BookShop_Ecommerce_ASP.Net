using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment.Models.DBO
{
    public class OrderViewModel
    {
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
    }
}
