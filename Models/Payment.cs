using System;

namespace QuanLyChoThuePhongTro.Models
{
    public class Payment
    {
        public int Id { get; set; }
        
        public int ContractId { get; set; }
        public RentalContract? Contract { get; set; }
        
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; } = "Bank Transfer"; // Bank Transfer, Cash, etc.
        
        // Status: Pending, Completed, Failed
        public string Status { get; set; } = "Pending";
        
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
