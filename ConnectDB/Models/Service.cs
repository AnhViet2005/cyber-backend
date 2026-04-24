using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectDB.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } // Drinks, Food, Hardware

        [MaxLength(50)]
        public string? SubCategory { get; set; } // Nước & Trộn, Cơm chiên, v.v.

        public string? ImageUrl { get; set; } // 👈 Ảnh thật của dịch vụ

        // Navigation property for area-specific pricing
        public ICollection<ServiceAreaPrice> AreaPrices { get; set; } = new List<ServiceAreaPrice>();

        public bool IsActive { get; set; } = true; // 👈 Trạng thái kinh doanh

        public int StockQuantity { get; set; } = 0; // 👈 Số lượng tồn kho

        public int? DurationMinutes { get; set; } // 👈 Thời lượng cộng thêm (đối với gói giờ chơi)
    }
}