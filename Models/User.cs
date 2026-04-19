using System;
using System.Collections.Generic;

namespace QuanLyChoThuePhongTro.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        
        // Role: Admin, Landlord, Tenant
        public string Role { get; set; } = "Tenant";
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Room>? OwnedRooms { get; set; }
        public virtual ICollection<RentalContract>? RentalContracts { get; set; }
    }
}
