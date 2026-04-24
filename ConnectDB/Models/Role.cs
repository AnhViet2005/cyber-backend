using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(20)]
        public string RoleName { get; set; } // Ví dụ: "Admin", "User"
    }
}
