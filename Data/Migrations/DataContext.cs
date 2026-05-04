using Microsoft.EntityFrameworkCore;
using Tweeting_book.Data;

namespace Tweeting_book.Migrations
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DbSet <Post> posts {get; set;}
    }
}