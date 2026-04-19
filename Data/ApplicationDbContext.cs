using Microsoft.EntityFrameworkCore;
using QuanLyChoThuePhongTro.Models;

namespace QuanLyChoThuePhongTro.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RentalContract> RentalContracts { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .HasMany(u => u.OwnedRooms)
                .WithOne(r => r.Owner)
                .HasForeignKey(r => r.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.RentalContracts)
                .WithOne(rc => rc.Tenant)
                .HasForeignKey(rc => rc.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Room configuration
            modelBuilder.Entity<Room>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Room>()
                .HasMany(r => r.RentalContracts)
                .WithOne(rc => rc.Room)
                .HasForeignKey(rc => rc.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // RentalContract configuration
            modelBuilder.Entity<RentalContract>()
                .HasKey(rc => rc.Id);

            modelBuilder.Entity<RentalContract>()
                .HasOne(rc => rc.Room)
                .WithMany(r => r.RentalContracts)
                .HasForeignKey(rc => rc.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RentalContract>()
                .HasOne(rc => rc.Tenant)
                .WithMany(u => u.RentalContracts)
                .HasForeignKey(rc => rc.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RentalContract>()
                .HasOne(rc => rc.Landlord)
                .WithMany()
                .HasForeignKey(rc => rc.LandlordId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment configuration
            modelBuilder.Entity<Payment>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Contract)
                .WithMany()
                .HasForeignKey(p => p.ContractId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
