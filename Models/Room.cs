using System;
using System.Collections.Generic;

namespace QuanLyChoThuePhongTro.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public float Area { get; set; }
        
        // Room details
        public int Bedrooms { get; set; }
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
