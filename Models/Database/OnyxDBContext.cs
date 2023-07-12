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

        public DbSet<Product> Products { get; set; }
    }
}
