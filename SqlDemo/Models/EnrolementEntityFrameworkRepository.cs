using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlDemo.Models
{
        public class EnrolementEntityFrameworkRepository : DbContext, IEnrolementRepository
    {
        public EnrolementEntityFrameworkRepository(DbContextOptions<EnrolementEntityFrameworkRepository> options)
         : base (options)
        { }

        public DbSet<Enrolement> Enrolement { get; set; }

        // use the fluent api to set up the Enrolement.ClassId & UserId fields as a composite primary key
        // https://stackoverflow.com/questions/40898365/asp-net-add-migration-composite-primary-key-error-how-to-use-fluent-api#40898681
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enrolement>()
                .HasKey(e => new { e.ClassId, e.UserId });
        }

        public void CreateEnrolement(Enrolement enrolement)
        {
            this.Enrolement.Add(enrolement);
            this.SaveChanges();
        }
        public IEnumerable<Enrolement> GetEnrolement()
        {
            return this.Enrolement;
        }
        public IEnumerable<Enrolement> FindEnrolementByClassId(Guid classId)
        {
            return this.Enrolement.Where(e => e.ClassId == classId);
        }
        public IEnumerable<Enrolement> FindEnrolementByUserId(Guid userId)
        {
            return this.Enrolement.Where(e => e.UserId == userId);
        }
        public void UpdateEnrolement(Enrolement enrolement)
        {
            this.Entry(enrolement).State = EntityState.Modified;
            this.SaveChanges();
        }
        public void DeleteEnrolement(Enrolement enrolement)
        {
            this.Enrolement.Remove(enrolement);
            this.SaveChanges();
        }
    }
}