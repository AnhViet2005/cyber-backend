using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectDB.Models
{
    public class Session
    {
        [Key]
        public int SessionId { get; set; }

        // FK -> Customer
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        // FK -> Computer
        public int ComputerId { get; set; }

        [ForeignKey("ComputerId")]
        public Computer? Computer { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; } // nullable vì chưa kết thúc

        [Column(TypeName = "decimal(10,2)")]
        public decimal HourlyRate { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; } // Playing / Done
    }
}