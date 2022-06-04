using Microsoft.EntityFrameworkCore;
using LearnWebApi.Models;


namespace LearnWebApi.Data
{
    public class DataContext : DbContext
    {
        #pragma warning disable CS8618
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        
        #pragma warning restore CS8618 
        public DbSet<User> User { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
    }
}
