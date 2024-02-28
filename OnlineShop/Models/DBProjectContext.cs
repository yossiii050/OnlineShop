using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineShop.Models
{
    public class DBProjectContext : IdentityDbContext<User>
    {
        public DBProjectContext(DbContextOptions<DBProjectContext> opts) : base(opts)
        {
            
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());

            builder.Entity<User>()
                .OwnsOne(u => u.Address, a =>
                {
                    a.Property(a => a.Street).HasColumnName("Street");
                    a.Property(a => a.City).HasColumnName("City");
                    a.Property(a => a.Country).HasColumnName("Country");
                    a.Property(a => a.ZipCode).HasColumnName("ZipCode");
                });
        }
        //Users
        public DbSet<Admin> admins { get; set; }
        public DbSet<User> User { get; set; }


        //Products
        public DbSet<Category> Category { get; set; }
        public DbSet<Item> Item { get; set; }



        //Orders
        public DbSet<Order> Order { get; set; }

        
    }
    public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u=>u.FirstName).HasMaxLength(255);
            builder.Property(u => u.LastName).HasMaxLength(255);

        }
    }
}
