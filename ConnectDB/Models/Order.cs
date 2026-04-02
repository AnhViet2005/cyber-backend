using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectDB.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        // FK -> Session
        public int SessionId { get; set; }

        [ForeignKey("SessionId")]
        public Session Session { get; set; }

        public DateTime OrderTime { get; set; } = DateTime.Now;
    }
}