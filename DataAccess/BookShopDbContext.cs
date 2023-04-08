using Assignment.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment.DataAccess
{
    public class BookShopDbContext :DbContext
    {
        public BookShopDbContext(DbContextOptions<BookShopDbContext> options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<User> Book { get; set; }
        public DbSet<User> Category { get; set; }
        public DbSet<User> Order { get; set; }
        public DbSet<User> OrderDetail { get; set; }

    }
}
