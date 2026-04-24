using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

        [Required]
        [MaxLength(100)]
        [JsonIgnore] // Để không trả về mật khẩu khi lấy danh sách khách hàng
        public string Password { get; set; } = "123456"; // Default password for seeding

        [Column(TypeName = "decimal(10,2)")]
        public decimal Balance { get; set; } = 0;

        public decimal RemainingTime { get; set; } = 0; // 👈 Số phút còn lại (tặng hoặc mua)

        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}