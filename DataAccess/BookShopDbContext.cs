using Assignment.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Assignment.DataAccess
{
    public class BookShopDbContext :IdentityDbContext<AppUser>
    {
        public BookShopDbContext(DbContextOptions<BookShopDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        //protected override void OnModelCreating(ModelBuilder builder)
        //{

        //    base.OnModelCreating(builder);
        //    // Bỏ tiền tố AspNet của các bảng: mặc định
        //    foreach (var entityType in builder.Model.GetEntityTypes())
        //    {
        //        var tableName = entityType.GetTableName();
        //        if (tableName.StartsWith("AspNet"))
        //        {
        //            entityType.SetTableName(tableName.Substring(6));
        //        }
        //    }
        //}

        public DbSet<User> User { get; set; }
        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }


    }
}
