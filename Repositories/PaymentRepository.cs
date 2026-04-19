using Microsoft.EntityFrameworkCore;
using QuanLyChoThuePhongTro.Data;
using QuanLyChoThuePhongTro.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLyChoThuePhongTro.Repositories
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<Payment?> GetByIdAsync(int id);
        Task<IEnumerable<Payment>> GetByContractAsync(int contractId);
        Task AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
        Task DeleteAsync(int id);
    }

    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Contract)
                    .ThenInclude(c => c!.Room)
                .Include(p => p.Contract)
                    .ThenInclude(c => c!.Tenant)
                .ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Contract)
                    .ThenInclude(c => c!.Room)
                .Include(p => p.Contract)
                    .ThenInclude(c => c!.Tenant)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Payment>> GetByContractAsync(int contractId)
        {
            return await _context.Payments
                .Where(p => p.ContractId == contractId)
                .Include(p => p.Contract)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task AddAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var payment = await GetByIdAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
