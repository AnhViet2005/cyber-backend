using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectDB.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        // FK -> Session
        public int SessionId { get; set; }

        [ForeignKey("SessionId")]
        public Session? Session { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(20)]
        public string PaymentMethod { get; set; } // Cash / Balance

        public DateTime PaymentTime { get; set; } = DateTime.UtcNow;
    }
}