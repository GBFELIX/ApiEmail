using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using APIEMAIL.Models;
using Microsoft.AspNetCore.Identity;
namespace APIEMAIL.Data
{
    public class AplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public AplicationDbContext(DbContextOptions<AplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Pessoa> Pessoas { get; set; } = null!;
    }
}

