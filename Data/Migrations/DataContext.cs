using Microsoft.EntityFrameworkCore;
using Tweeting_book.Data;
using Tweeting_book.Domain;

namespace Tweeting_book.Migrations
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DbSet <Post> posts {get; set;}

        public DbSet<RefreshToken> RefreshTokens{get; set;}
    }
}