using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Models
{
    public class DBUsers : DbContext
    {
        public DBUsers(DbContextOptions<DBUsers> opts) : base(opts)
        {
            
        }

        public DbSet<User> User { get; set; }
        public DbSet<Order> Order { get; set; }
		public DbSet<Category> Category { get; set; }
		public DbSet<Item> Item { get; set; }
	}
}
