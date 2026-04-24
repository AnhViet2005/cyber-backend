using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConnectDB.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        public string? Name { get; set; }
        public string? Type { get; set; }
        public decimal PricePerHour { get; set; }
        public string? ImageUrl { get; set; }

        [JsonIgnore]
        public ICollection<Computer>? Computers { get; set; } // 👈 QUAN TRỌNG: thêm ?
    }
}