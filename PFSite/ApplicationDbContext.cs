using Microsoft.EntityFrameworkCore;
using PFSite.Models;

namespace PFSite.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {
        }

        public DbSet<Record> Records { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PointLog> PointLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //var record = modelBuilder.Entity<Record>();
            var user = modelBuilder.Entity<User>();
            user.HasKey("StudentId");
            user.HasIndex("GitHubId").IsUnique();
        }
    }
}
