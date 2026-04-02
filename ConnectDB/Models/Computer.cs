using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectDB.Models
{
    public class Computer
    {
        [Key]
        public int ComputerId { get; set; }

        [Required]
        public string ComputerName { get; set; }

        public string Status { get; set; }

        // Foreign key
        public int RoomId { get; set; }

        // Navigation property
        [ForeignKey("RoomId")]
        public Room? Room { get; set; }
    }
}