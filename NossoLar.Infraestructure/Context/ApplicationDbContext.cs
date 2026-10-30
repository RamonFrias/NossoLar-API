using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NossoLar.Domain.MonthlyBudgets.Entities;
using NossoLar.Domain.Residentials.Entities;
using NossoLar.Infraestructure.Identity;

namespace NossoLar.Infraestructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options) { }

        public DbSet<Residential> Residentials { get; set; }
        public DbSet<MonthlyBudget> MonthlyBudgets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
