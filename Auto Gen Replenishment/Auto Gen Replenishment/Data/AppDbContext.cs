using Microsoft.EntityFrameworkCore;
using AutoGenReplenishment.Models;

namespace AutoGenReplenishment.Data
{
    internal class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        //protected AppDbContext()
        //{
        //}

        public DbSet<ReplenishmentHdr> Replenishments => Set<ReplenishmentHdr>();
        public DbSet<ReplenishmentItemsByMinimum> ReplenishmentItemsByMinimums => Set<ReplenishmentItemsByMinimum>();
        public DbSet<Company> Companies => Set<Company>();
        // public DbSet<Company> Companies { get; set; }
    }
}
