using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShop.Models.Cart;
using OnlineShop.Models.ChatBot;
using OnlineShop.Models.Message;

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

			builder.Entity<Product>()
				.HasOne(p => p.Category)
				.WithMany(c => c.Products)
				.HasForeignKey(p => p.CategoryId);

            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .IsRequired(false);


            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);


        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<CreditCard> CreditCards { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }


        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<UserPromoCode> UserPromoCodes { get; set; }


        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<MessageInbox> messages { get; set; }
        public DbSet<ProductNotification> ProductNotifications { get; set; }


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

