using Microsoft.EntityFrameworkCore;
using QuanLyChoThuePhongTro.Data;
using QuanLyChoThuePhongTro.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyChoThuePhongTro.Repositories
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllAsync();
        Task<Room?> GetByIdAsync(int id);
        Task<IEnumerable<Room>> SearchAsync(RoomFilter filter);
        Task<IEnumerable<Room>> GetByOwnerAsync(int ownerId);
        Task AddAsync(Room room);
        Task UpdateAsync(Room room);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }

    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _context.Rooms
                .Include(r => r.Owner)
                .ToListAsync();
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            return await _context.Rooms
                .Include(r => r.Owner)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Room>> SearchAsync(RoomFilter filter)
        {
            var query = _context.Rooms.AsQueryable();

            // Normalize inputs
            var searchQuery = filter?.SearchQuery?.Trim();
            var district = filter?.District?.Trim();
            var ward = filter?.Ward?.Trim();
            var status = filter?.Status?.Trim();

            var hasSearchQuery = !string.IsNullOrEmpty(searchQuery);
            var hasDistrict = !string.IsNullOrEmpty(district);
            var hasWard = !string.IsNullOrEmpty(ward);
            var hasStatus = !string.IsNullOrEmpty(status);

            // Apply text filters independently (không ph? thu?c vào MinPrice)
            if (hasSearchQuery)
            {
                var keyword = $"%{searchQuery}%";
                query = query.Where(r =>
                    EF.Functions.ILike(r.Title, keyword) ||
                    EF.Functions.ILike(r.Description, keyword) ||
                    EF.Functions.ILike(r.Location, keyword));
            }

            if (hasDistrict)
            {
                var districtKeyword = $"%{district}%";
                query = query.Where(r => EF.Functions.ILike(r.District, districtKeyword));
            }

            if (hasWard)
            {
                var wardKeyword = $"%{ward}%";
                query = query.Where(r => EF.Functions.ILike(r.Ward, wardKeyword));
            }

            if (hasStatus)
            {
                query = query.Where(r => EF.Functions.ILike(r.Status, status!));
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(r => r.Price >= filter.MinPrice.Value);
            }

            if (filter?.MaxPrice.HasValue == true)
            {
                query = query.Where(r => r.Price <= filter.MaxPrice.Value);
            }

            if (filter.MinArea.HasValue)
            {
                query = query.Where(r => r.Area >= filter.MinArea.Value);
            }

            if (filter.MaxArea.HasValue)
            {
                query = query.Where(r => r.Area <= filter.MaxArea.Value);
            }

            if (filter.Bedrooms.HasValue)
            {
                query = query.Where(r => r.Bedrooms >= filter.Bedrooms.Value);
            }

            if (filter.Bathrooms.HasValue)
            {
                query = query.Where(r => r.Bathrooms >= filter.Bathrooms.Value);
            }

            if (filter.HasKitchen.HasValue && filter.HasKitchen.Value)
            {
                query = query.Where(r => r.HasKitchen == true);
            }

            if (filter.HasWiFi.HasValue && filter.HasWiFi.Value)
            {
                query = query.Where(r => r.HasWiFi == true);
            }

            if (filter.HasAirConditioner.HasValue && filter.HasAirConditioner.Value)
            {
                query = query.Where(r => r.HasAirConditioner == true);
            }

            if (filter.HasWashing.HasValue && filter.HasWashing.Value)
            {
                query = query.Where(r => r.HasWashing == true);
            }

            return await query.Include(r => r.Owner).ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetByOwnerAsync(int ownerId)
        {
            return await _context.Rooms
                .Where(r => r.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task AddAsync(Room room)
        {
            _context.Rooms.Add(room);
            await SaveAsync();
        }

        public async Task UpdateAsync(Room room)
        {
            _context.Rooms.Update(room);
            await SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var room = await GetByIdAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await SaveAsync();
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
