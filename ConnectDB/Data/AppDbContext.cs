using Microsoft.EntityFrameworkCore;
using ConnectDB.Models;

namespace ConnectDB.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSet
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Computer> Computers { get; set; }
        public DbSet<ServiceAreaPrice> ServiceAreaPrices { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SupportMessage> SupportMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UNIQUE Username
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Username)
                .IsUnique();

            // Relationships
            modelBuilder.Entity<Computer>()
                .HasOne(c => c.Room)
                .WithMany(r => r.Computers)
                .HasForeignKey(c => c.RoomId);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Customer)
                .WithMany()
                .HasForeignKey(s => s.CustomerId);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Computer)
                .WithMany()
                .HasForeignKey(s => s.ComputerId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Session)
                .WithMany()
                .HasForeignKey(o => o.SessionId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Service)
                .WithMany()
                .HasForeignKey(od => od.ServiceId);

            modelBuilder.Entity<Payment>()
            .HasOne(p => p.Session)
            .WithOne()
            .HasForeignKey<Payment>(p => p.SessionId);

        // Quan hệ Customer - Role
        modelBuilder.Entity<Customer>()
            .HasOne(c => c.Role)
            .WithMany()
            .HasForeignKey(c => c.RoleId);

        // Quan hệ SupportMessage - Customer
        modelBuilder.Entity<SupportMessage>()
            .HasOne(m => m.Customer)
            .WithMany()
            .HasForeignKey(m => m.CustomerId);

        // Seed Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { RoleId = 1, RoleName = "Admin" },
            new Role { RoleId = 2, RoleName = "User" }
        );
    }
    }
}