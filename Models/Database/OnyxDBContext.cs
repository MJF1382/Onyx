using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Onyx.Models.Database.Entities;

namespace Onyx.Models.Database
{
    public class OnyxDBContext : IdentityDbContext
    {
        public OnyxDBContext()
        {

        }

        public OnyxDBContext(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=(local);Database=OnyxDB;Trusted_Connection=True");
        }

        public DbSet<Product> Products { get; set; }
    }
}
