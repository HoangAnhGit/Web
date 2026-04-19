using System;

namespace QuanLyChoThuePhongTro.Models
{
    public class RentalContract
    {
        public int Id { get; set; }
        
        public int RoomId { get; set; }
        public Room? Room { get; set; }
        
        public int TenantId { get; set; }
        public User? Tenant { get; set; }
        
        public int LandlordId { get; set; }
        public User? Landlord { get; set; }
        
        public decimal MonthlyPrice { get; set; }
        public decimal Deposit { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        // Status: Active, Expired, Terminated, Pending
        public string Status { get; set; } = "Pending";
        
        public string TermsAndConditions { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        
        // Payment tracking
        public DateTime? LastPaymentDate { get; set; }
        public decimal RemainingDeposit { get; set; }
    }
}
