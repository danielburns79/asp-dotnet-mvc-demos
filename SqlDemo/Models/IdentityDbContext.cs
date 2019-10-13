using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace SqlDemo.Models
{
    // UserEntityFrameworkRepository uses entity framework -- see UserSqlRepository for direct sql query example
    public class IdentityDbContext : DbContext, IIdentityUserRepository
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

        public void CreateUser(IdentityUser<Guid> user)
        {
            this.IdentityUser.Add(user);
            this.SaveChanges();
        }
        public IEnumerable<IdentityUser<Guid>> GetUsers()
        {
            return this.IdentityUser;
        }
        public IdentityUser<Guid> GetUser(Guid userId)
        {
            return this.IdentityUser.Find(userId);
        }
        public IEnumerable<IdentityUser<Guid>> FindUsersByFirstAndOrLastName(string firstAndOrLastName)
        {
            return this.IdentityUser.Where(iu => iu.UserName.Contains(firstAndOrLastName));
        }
        public IEnumerable<IdentityUser<Guid>> FindUsersByEmail(string email)
        {
            return this.IdentityUser.Where(iu => iu.NormalizedEmail == email.ToUpper());
        }
        public IEnumerable<IdentityUser<Guid>> FindUsersByClass(Guid classId, IEnrolementRepository enrolementRepository)
        {
            // must pull one of the tables into local memory with .ToList() before executing the join
            // otherwise, entity framework will throw an argumentnullexception
            // calling .ToList() on users executes the query
            /*return (
                from u in this.IdentityUser.ToList()
                join e in enrolementRepository.Enrolement
                on u.Id equals e.UserId
                where e.ClassId == classId
                select new IdentityUser<Guid> {
                    Id = u.Id, 
                    Email = u.Email, 
                    UserName = u.UserName
                    }
                ).ToList();*/
            // the following does the same as the above but using the lambda syntax instead of the linq syntax
            /* return this.IdentityUser.ToList().Join(
                enrolementRepository.Enrolement, 
                u => u.Id, 
                e => e.UserId, 
                (u, e) => new IdentityUser<Guid>{Id = u.Id, Email = u.Email, UserName = u.UserName}
                ).ToList();*/
            // the following executes the join on the sql server instead of pulling the table into local memory and then doing a linq join
            IdentityUserSqlRepository userSqlRepository = new IdentityUserSqlRepository(this.Database.GetDbConnection().ConnectionString);
            return userSqlRepository.FindUsersByClass(classId, enrolementRepository);
        }
        public void UpdateUser(IdentityUser<Guid> user)
        {
            this.Entry(user).State = EntityState.Modified;
            this.SaveChanges();
        }
        public void DeleteUser(Guid userId)
        {
            var user = this.IdentityUser.Find(userId);
            if (user == null)
            {
                throw new InvalidOperationException("failed to delete user");
            }
            this.IdentityUser.Remove(user);
            this.SaveChanges();
        }     
    }
}