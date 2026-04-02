using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConnectDB.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        public string? Name { get; set; }   // 👈 nên nullable

        public string? Type { get; set; }   // 👈 nên nullable

        [JsonIgnore] // 👈 tránh vòng lặp JSON
        public ICollection<Computer>? Computers { get; set; } // 👈 QUAN TRỌNG: thêm ?
    }
}