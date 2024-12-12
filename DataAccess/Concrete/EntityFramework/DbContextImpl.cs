using Core.Entities.Concrete;
using Entity.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class DbContextImpl : DbContext
    {
        protected IConfiguration Configuration { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-NC93FPA;Database=Cafeteriadb;User Id=sa;Password=123456");
            //optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Branch>()
                .HasOne(b => b.Municipality)
                .WithMany(m => m.Branches)
                .HasForeignKey(b => b.MunicipalityId);
            modelBuilder.Entity<Product>()
                .Property(o => o.UnitPrice)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);
            modelBuilder.Entity<UserOperationClaim>().HasKey(uo => new { uo.UserId, uo.OperationClaimId });
            modelBuilder.Entity<OrderProduct>().HasKey(op => new { op.OrderId, op.ProductId });
        }
        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Municipality> Municipalities { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Branch> Branches { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<OperationClaim> OperationClaims { get; set; }

        public DbSet<UserOperationClaim> UserOperationClaims { get; set; }

        public DbSet<OrderProduct> OrderProducts { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
