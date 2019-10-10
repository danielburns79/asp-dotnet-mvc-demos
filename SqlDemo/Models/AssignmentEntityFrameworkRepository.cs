using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlDemo.Models
{
    public class AssignmentEntityFrameworkRepository : DbContext, IAssignmentRepository
    {
        public AssignmentEntityFrameworkRepository(DbContextOptions<AssignmentEntityFrameworkRepository> options)
         : base (options)
        { }

        public DbSet<Assignment> Assignments { get; set; }

        // use the fluent api to set up the Assignment.AssignmentId & ProblemId fields as a composite primary key
        // https://stackoverflow.com/questions/40898365/asp-net-add-migration-composite-primary-key-error-how-to-use-fluent-api#40898681
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assignment>()
                .HasKey(a => new { a.AssignmentId, a.ProblemId });
        }

        public void CreateAssignment(Assignment assignmentEntity)
        {
            this.Assignments.Add(assignmentEntity);
            this.SaveChanges();
        }
        public IEnumerable<Assignment> GetAssignments()
        {
            return this.Assignments;
        }
        public Assignment GetAssignment(Guid assignmentId)
        {
            return this.Assignments.Find(assignmentId);
        }
        public IEnumerable<Assignment> FindAssignmentsByClass(Guid classId, IProblemRepository problemRepository)
        {
            return (
                from a in this.Assignments.ToList()
                join p in problemRepository.Problems
                on a.ProblemId equals p.ProblemId
                where p.ClassId == classId
                select a
            ).ToList();
        }
        public IEnumerable<Assignment> FindAssignmentsByStudent(Guid userId, IProblemRepository problemRepository)
        {
            return (
                from a in this.Assignments.ToList()
                join p in problemRepository.Problems
                on a.ProblemId equals p.ProblemId
                where p.StudentId == userId
                select a
            ).ToList();
        }
        public IEnumerable<Assignment> FindAssignmentsByTeacher(Guid userId, IProblemRepository problemRepository, IEnrolementRepository enrolementRepository)
        {
            return (
                from a in this.Assignments.ToList()
                join p in problemRepository.Problems
                on a.ProblemId equals p.ProblemId
                join e in enrolementRepository.Enrolement
                on p.ClassId equals e.ClassId
                where e.UserId == userId && e.Role == 2
                select a
            ).ToList();
        }
        public void UpdateAssignment(Assignment assignmentEntity)
        {
            this.Entry(assignmentEntity).State = EntityState.Modified;
            this.SaveChanges();
        }
        public void DeleteAssignment(Guid assignmentId)
        {
            var assignmentEntity = this.Assignments.Find(assignmentId);
            if (assignmentEntity == null)
            {
                throw new InvalidOperationException("failed to delete Assignment");
            }
            this.Assignments.Remove(assignmentEntity);
            this.SaveChanges();
        }
    }
}