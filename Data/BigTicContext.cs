using BigTic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace BigTic.Data
{
    public class BigTicContext : IdentityDbContext<Auth, IdentityRole<long>, long>
    {
        public BigTicContext(DbContextOptions<BigTicContext> options)
            : base(options)
        {
        }
        public DbSet<Auth> Auths { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Auth>().ToTable("Auth");
        }
    }
}
