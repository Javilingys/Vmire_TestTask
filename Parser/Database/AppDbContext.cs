using Microsoft.EntityFrameworkCore;
using Parser.Database.Configurations;
using Parser.Models;

namespace Parser.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {
        }

        public DbSet<NumberCount> NumberCounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(NumberCountConfiguration).Assembly);
        }
    }
}
