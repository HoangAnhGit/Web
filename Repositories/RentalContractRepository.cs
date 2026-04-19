using Microsoft.EntityFrameworkCore;
using QuanLyChoThuePhongTro.Data;
using QuanLyChoThuePhongTro.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyChoThuePhongTro.Repositories
{
    public interface IRentalContractRepository
    {
        Task<IEnumerable<RentalContract>> GetAllAsync();
        Task<RentalContract?> GetByIdAsync(int id);
        Task<IEnumerable<RentalContract>> GetByTenantAsync(int tenantId);
        Task<IEnumerable<RentalContract>> GetByLandlordAsync(int landlordId);
        Task<IEnumerable<RentalContract>> GetByRoomAsync(int roomId);
        Task AddAsync(RentalContract contract);
        Task UpdateAsync(RentalContract contract);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }

    public class RentalContractRepository : IRentalContractRepository
    {
        private readonly ApplicationDbContext _context;

        public RentalContractRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RentalContract>> GetAllAsync()
        {
            return await _context.RentalContracts
                .Include(rc => rc.Room)
                .Include(rc => rc.Tenant)
                .Include(rc => rc.Landlord)
                .ToListAsync();
        }

        public async Task<RentalContract?> GetByIdAsync(int id)
        {
            return await _context.RentalContracts
                .Include(rc => rc.Room)
                .Include(rc => rc.Tenant)
                .Include(rc => rc.Landlord)
                .FirstOrDefaultAsync(rc => rc.Id == id);
        }

        public async Task<IEnumerable<RentalContract>> GetByTenantAsync(int tenantId)
        {
            return await _context.RentalContracts
                .Where(rc => rc.TenantId == tenantId)
                .Include(rc => rc.Room)
                .Include(rc => rc.Landlord)
                .ToListAsync();
        }

        public async Task<IEnumerable<RentalContract>> GetByLandlordAsync(int landlordId)
        {
            return await _context.RentalContracts
                .Where(rc => rc.LandlordId == landlordId)
                .Include(rc => rc.Room)
                .Include(rc => rc.Tenant)
                .ToListAsync();
        }

        public async Task<IEnumerable<RentalContract>> GetByRoomAsync(int roomId)
        {
            return await _context.RentalContracts
                .Where(rc => rc.RoomId == roomId)
                .ToListAsync();
        }

        public async Task AddAsync(RentalContract contract)
        {
            _context.RentalContracts.Add(contract);
            await SaveAsync();
        }

        public async Task UpdateAsync(RentalContract contract)
        {
            var local = _context.RentalContracts.Local.FirstOrDefault(e => e.Id == contract.Id);
            if (local != null && !ReferenceEquals(local, contract))
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.RentalContracts.Update(contract);
            await SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var contract = await GetByIdAsync(id);
            if (contract != null)
            {
                _context.RentalContracts.Remove(contract);
                await SaveAsync();
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
