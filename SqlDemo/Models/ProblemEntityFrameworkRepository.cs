using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlDemo.Models
{
    public class ProblemEntityFrameworkRepository : DbContext, IProblemRepository
    {
        public ProblemEntityFrameworkRepository(DbContextOptions<ProblemEntityFrameworkRepository> options)
         : base (options)
        { }

        public DbSet<Problem> Problems { get; set; }

        public void CreateProblem(Problem problemEntity)
        {
            this.Problems.Add(problemEntity);
            this.SaveChanges();
        }
        public IEnumerable<Problem> GetProblems()
        {
            return this.Problems;
        }
        public Problem GetProblem(Guid problemId)
        {
            return this.Problems.Find(problemId);
        }
        public IEnumerable<Problem> FindProblemsByQuestion(Guid questionId)
        {
            return (
                from p in this.Problems
                where p.QuestionId == questionId
                select p
            ).ToList();
        }
        public IEnumerable<Problem> FindProblemsByClass(Guid classId)
        {
            return (
                from p in this.Problems
                where p.ClassId == classId
                select p
            ).ToList();
        }
        public IEnumerable<Problem> FindProblemsByStudent(Guid userId)
        {
            return (
                from p in this.Problems
                where p.StudentId == userId
                select p
            ).ToList();
        }
        public IEnumerable<Problem> FindProblemsByTeacher(Guid userId, IEnrolementRepository enrolementRepository)
        {
            // only implementing the linq version, see UserEntityFrameworkRepository.FindUsersByClass for SQL implementation
            return (
                from p in this.Problems.ToList()
                join e in enrolementRepository.Enrolement
                on p.ClassId equals e.ClassId
                where e.UserId == userId && e.Role == 2
                select p
            ).ToList();
        }
        public IEnumerable<Problem> FindProblemsByAssignment(Guid assignmentId, IAssignmentRepository assignmentRepository)
        {
            // only implementing the linq version, see UserEntityFrameworkRepository.FindUsersByClass for SQL implementation
            return (
                from p in this.Problems.ToList()
                join a in assignmentRepository.Assignments
                on p.ProblemId equals a.ProblemId
                where a.AssignmentId == assignmentId
                select p
            ).ToList();
        }
        public void UpdateProblem(Problem problemEntity)
        {
            this.Entry(problemEntity).State = EntityState.Modified;
            this.SaveChanges();
        }
        public void DeleteProblem(Guid problemId)
        {
            var problemEntity = this.Problems.Find(problemId);
            if (problemEntity == null)
            {
                throw new InvalidOperationException("failed to delete Problem");
            }
            this.Problems.Remove(problemEntity);
            this.SaveChanges();
        }
    }
}