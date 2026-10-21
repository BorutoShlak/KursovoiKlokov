using Microsoft.EntityFrameworkCore;

namespace PostOfficeAPI.Models
{
    public class PostOfficeContext : DbContext
    {
        public PostOfficeContext(DbContextOptions<PostOfficeContext> options) : base(options)
        {
        }

        public DbSet<Pochta_Clients> Pochta_Clients { get; set; }
        public DbSet<Pochta_Employees> Pochta_Employees { get; set; }
        public DbSet<Pochta_Shipments> Pochta_Shipments { get; set; }
        public DbSet<Pochta_Tarrifs> Pochta_Tarrifs { get; set; }
        public DbSet<Pochta_Statuses> Pochta_Statuses { get; set; }
        public DbSet<Pochta_StatusHistory> Pochta_StatusHistory { get; set; }
        public DbSet<Pochta_Users> Pochta_Users { get; set; }
        public DbSet<Pochta_Dolzhonst> Pochta_Dolzhonst { get; set; }
        public DbSet<Pochta_Role> Pochta_Role { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Pochta_Shipments>()
                .HasOne(s => s.Pochta_Clients).WithMany(c => c.Pochta_Shipments).HasForeignKey(s => s.sender).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pochta_Shipments>().HasOne(s => s.Pochta_Clients1).WithMany(c => c.Pochta_Shipments1).HasForeignKey(s => s.receiver).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pochta_Shipments>().HasOne(s => s.Pochta_Tarrifs).WithMany(t => t.Pochta_Shipments).HasForeignKey(s => s.tarif);

            modelBuilder.Entity<Pochta_Shipments>().HasOne(s => s.Pochta_Statuses).WithMany(st => st.Pochta_Shipments).HasForeignKey(s => s.status);

            modelBuilder.Entity<Pochta_Shipments>().HasOne(s => s.Pochta_Employees).WithMany(e => e.Pochta_Shipments).HasForeignKey(s => s.employee);

            modelBuilder.Entity<Pochta_Employees>().HasOne(e => e.Pochta_Dolzhonst).WithMany(d => d.Pochta_Employees).HasForeignKey(e => e.dolzhnost);

            modelBuilder.Entity<Pochta_Users>().HasOne(u => u.Pochta_Employees).WithMany(e => e.Pochta_Users).HasForeignKey(u => u.employee);

            modelBuilder.Entity<Pochta_Users>().HasOne(u => u.Pochta_Role).WithMany(r => r.Pochta_Users).HasForeignKey(u => u.role);

            modelBuilder.Entity<Pochta_StatusHistory>().HasOne(h => h.Pochta_Shipments).WithMany(s => s.Pochta_StatusHistory).HasForeignKey(h => h.shipment);

            modelBuilder.Entity<Pochta_StatusHistory>().HasOne(h => h.Pochta_Statuses).WithMany(st => st.Pochta_StatusHistory).HasForeignKey(h => h.status);
        }
    }
}