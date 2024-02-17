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
    }
}
