using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLyChoThuePhongTro.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu ?? là b?t bu?c.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô t? là b?t bu?c.")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá thuê ph?i l?n h?n 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "??a ch? là b?t bu?c.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Qu?n là b?t bu?c.")]
        public string District { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ph??ng là b?t bu?c.")]
        public string Ward { get; set; } = string.Empty;

        [Range(0.1, double.MaxValue, ErrorMessage = "Di?n tích ph?i l?n h?n 0.")]
        public float Area { get; set; }
        
        // Room details
        [Range(0, int.MaxValue, ErrorMessage = "S? phòng ng? không h?p l?.")]
        public int Bedrooms { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "S? phòng t?m không h?p l?.")]
        public int Bathrooms { get; set; }
        public bool HasKitchen { get; set; }
        public bool HasWiFi { get; set; }
        public bool HasAirConditioner { get; set; }
        public bool HasWashing { get; set; }
        
        // Status: Available, Rented, Maintenance
        public string Status { get; set; } = "Available";
        
        public int OwnerId { get; set; }
        public User? Owner { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        
        // Image paths (comma-separated)
        public string? ImageUrls { get; set; }

        // Navigation properties
        public virtual ICollection<RentalContract>? RentalContracts { get; set; }
    }
}
