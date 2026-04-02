using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectDB.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [MaxLength(100)]
        public string Fullname { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Balance { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}