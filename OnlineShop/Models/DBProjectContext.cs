using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Models
{
    public class DBProjectContext : DbContext
    {
        public DBProjectContext(DbContextOptions<DBProjectContext> opts) : base(opts)
        {
            
        }

        //Users
        public DbSet<Admin> admins { get; set; }
        public DbSet<User> User { get; set; }


        //Products
        public DbSet<Category> Category { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<Product> Product { get; set; }



        //Orders
        public DbSet<Order> Order { get; set; }

	}
}
