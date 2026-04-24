using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ConnectDB.Models
{
    public class ServiceAreaPrice
    {
        [Key]
        public int ServiceAreaPriceId { get; set; }

        public int ServiceId { get; set; }

        [Required]
        [MaxLength(50)]
        public string AreaName { get; set; } // Vip, Standard, Stream

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        // Navigation property
        [JsonIgnore]
        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }
    }
}
