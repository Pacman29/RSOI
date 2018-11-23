using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Contexts
{
    public class BaseContext : IdentityDbContext
    {
        public BaseContext(DbContextOptions options) : base(options)
        {
       
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        
    }
}