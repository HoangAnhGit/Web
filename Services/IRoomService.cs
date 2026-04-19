using QuanLyChoThuePhongTro.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLyChoThuePhongTro.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task<Room?> GetRoomByIdAsync(int id);
        Task<IEnumerable<Room>> SearchRoomsAsync(RoomFilter filter);
        Task<IEnumerable<Room>> GetRoomsByOwnerAsync(int ownerId);
        Task AddRoomAsync(Room room);
        Task UpdateRoomAsync(Room room);
        Task DeleteRoomAsync(int id);
        Task ClearCacheAsync();
    }
}