using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace SqlDemo.Models
{
    // UserEntityFrameworkRepository uses entity framework -- see UserSqlRepository for direct sql query example
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
         : base (options)
        { }

        public DbSet<IdentityUser<Guid>> IdentityUser { get; set; }
        public DbSet<IdentityUserClaim<Guid>> IdentityUserClaim { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserRole<Guid>>()
                .HasKey(i => new { i.RoleId, i.UserId });
        }
        public DbSet<IdentityUserRole<Guid>> IdentityUserRole { get; set; }

        public DbSet<IdentityRole<Guid>> IdentityRole { get; set; }
    }
}