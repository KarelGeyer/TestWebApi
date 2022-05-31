using Microsoft.EntityFrameworkCore;
using LearnWebApi.Models;


namespace LearnWebApi.Data
{
    public class DataContext : DbContext
    {
#pragma warning disable CS8618 // Pole, které nemůže být null, musí při ukončování konstruktoru obsahovat hodnotu, která není null. Zvažte možnost deklarovat ho jako pole s možnou hodnotou null.
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
#pragma warning restore CS8618 // Pole, které nemůže být null, musí při ukončování konstruktoru obsahovat hodnotu, která není null. Zvažte možnost deklarovat ho jako pole s možnou hodnotou null.
        public DbSet<User> User { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
    }
}
