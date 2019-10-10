using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlDemo.Models
{
    public class ClassEntityFrameworkRepository : DbContext, IClassRepository
    {
        public ClassEntityFrameworkRepository(DbContextOptions<ClassEntityFrameworkRepository> options)
         : base (options)
        { }

        public DbSet<Class> Classes { get; set; }

        public void CreateClass(Class classEntity)
        {
            this.Classes.Add(classEntity);
            this.SaveChanges();
        }
        public IEnumerable<Class> GetClasses()
        {
            return this.Classes;
        }
        public Class GetClass(Guid classId)
        {
            return this.Classes.Find(classId);
        }
        public IEnumerable<Class> GetClasses(string className)
        {
            return this.Classes.Where(c => c.ClassName.Contains(className));
        }
        public void UpdateClass(Class classEntity)
        {
            this.Entry(classEntity).State = EntityState.Modified;
            this.SaveChanges();
        }
        public void DeleteClass(Guid classId)
        {
            var classEntity = this.Classes.Find(classId);
            if (classEntity == null)
            {
                throw new InvalidOperationException("failed to delete Class");
            }
            this.Classes.Remove(classEntity);
            this.SaveChanges();
        }
    }
}