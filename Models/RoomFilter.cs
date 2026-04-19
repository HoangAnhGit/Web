using System;

namespace QuanLyChoThuePhongTro.Models
{
    public class RoomFilter
    {
        public string? SearchQuery { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public float? MinArea { get; set; }
        public float? MaxArea { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public bool? HasKitchen { get; set; }
        public bool? HasWiFi { get; set; }
        public bool? HasAirConditioner { get; set; }
        public bool? HasWashing { get; set; }
        public string? Status { get; set; }
    }
}
