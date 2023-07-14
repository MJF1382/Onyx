using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Onyx.Models.Database.Entities;
using Onyx.Models.Identity.Entities;

namespace Onyx.Models.Database
{
    public class OnyxDBContext : IdentityDbContext<AppUser>
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
