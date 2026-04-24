using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectDB.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        // FK -> Session
        public int? SessionId { get; set; }

        [ForeignKey("SessionId")]
        public Session? Session { get; set; }

        public DateTime OrderTime { get; set; } = DateTime.UtcNow;
        
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Preparing, Delivering, Completed, Cancelled

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public bool IsAdminOrder { get; set; } = false;

        public string? Note { get; set; } // 👈 Ghi chú cho bếp/nhân viên
    }
}