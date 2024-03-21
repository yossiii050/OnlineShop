using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShop.Models.Cart;

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

			// Configure the relationship between Product and Category
			builder.Entity<Product>()
				.HasOne(p => p.Category)
				.WithMany(c => c.Products)
				.HasForeignKey(p => p.CategoryId);

            // Configure the relationship between Order and User
            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            // Configure the relationship between OrderItem and Order
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            // Configure the relationship between OrderItem and Product
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);
        }
        //Users
        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }


        //Products
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }


        public DbSet<CartItem> CartItems { get; set; }

        //Orders
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }


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

