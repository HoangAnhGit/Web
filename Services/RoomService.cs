using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using QuanLyChoThuePhongTro.Models;
using QuanLyChoThuePhongTro.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyChoThuePhongTro.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _repository;
        private readonly IMemoryCache _cache;
        private const string CACHE_KEY_ALL_ROOMS = "all_rooms";
        private const string CACHE_KEY_ROOM = "room_";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

        public RoomService(IRoomRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            if (_cache.TryGetValue(CACHE_KEY_ALL_ROOMS, out IEnumerable<Room>? rooms))
            {
                return rooms!;
            }

            var roomsFromDb = await _repository.GetAllAsync();
            _cache.Set(CACHE_KEY_ALL_ROOMS, roomsFromDb, _cacheExpiration);

            return roomsFromDb;
        }

        public async Task<Room?> GetRoomByIdAsync(int id)
        {
            var cacheKey = $"{CACHE_KEY_ROOM}{id}";

            if (_cache.TryGetValue(cacheKey, out Room? room))
            {
                return room;
            }

            var roomFromDb = await _repository.GetByIdAsync(id);
            if (roomFromDb != null)
            {
                _cache.Set(cacheKey, roomFromDb, _cacheExpiration);
            }

            return roomFromDb;
        }

        public async Task<IEnumerable<Room>> SearchRoomsAsync(RoomFilter filter)
        {
            return await _repository.SearchAsync(filter);
        }

        public async Task<IEnumerable<Room>> GetRoomsByOwnerAsync(int ownerId)
        {
            return await _repository.GetByOwnerAsync(ownerId);
        }

        public async Task AddRoomAsync(Room room)
        {
            await _repository.AddAsync(room);
            await ClearCacheAsync();
        }

        public async Task UpdateRoomAsync(Room room)
        {
            await _repository.UpdateAsync(room);
            _cache.Remove($"{CACHE_KEY_ROOM}{room.Id}");
            _cache.Remove(CACHE_KEY_ALL_ROOMS);
        }

        public async Task DeleteRoomAsync(int id)
        {
            await _repository.DeleteAsync(id);
            _cache.Remove($"{CACHE_KEY_ROOM}{id}");
            _cache.Remove(CACHE_KEY_ALL_ROOMS);
        }

        public async Task ClearCacheAsync()
        {
            _cache.Remove(CACHE_KEY_ALL_ROOMS);
            await Task.CompletedTask;
        }
    }
}
