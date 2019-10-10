using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlDemo.Models
{
    public class QuestionEntityFrameworkRepository : DbContext, IQuestionRepository
    {
        public QuestionEntityFrameworkRepository(DbContextOptions<QuestionEntityFrameworkRepository> options)
         : base (options)
        { }

        public DbSet<Question> Questions { get; set; }

        public void CreateQuestion(Question questionEntity)
        {
            this.Questions.Add(questionEntity);
            this.SaveChanges();
        }
        public IEnumerable<Question> GetQuestions()
        {
            return this.Questions;
        }
        public Question GetQuestion(Guid questionId)
        {
            return this.Questions.Find(questionId);
        }
        public IEnumerable<Question> GetQuestions(string question)
        {
            return this.Questions.Where(q => q.QuestionString.Contains(question));
        }
        public IEnumerable<Question> FindQuestionsByClass(Guid classId, IProblemRepository problemRepository)
        {
            return (
                from q in this.Questions.ToList()
                join p in problemRepository.Problems
                on q.QuestionId equals p.QuestionId
                where p.ClassId == classId
                select q
            ).ToList();
        }
        public IEnumerable<Question> FindQuestionsByStudent(Guid userId, IProblemRepository problemRepository)
        {
            return (
                from q in this.Questions.ToList()
                join p in problemRepository.Problems
                on q.QuestionId equals p.QuestionId
                where p.StudentId == userId
                select q
            ).ToList();
        }
        public IEnumerable<Question> FindQuestionsByTeacher(Guid userId, IProblemRepository problemRepository, IEnrolementRepository enrolementRepository)
        {
            return (
                from q in this.Questions.ToList()
                join p in problemRepository.Problems
                on q.QuestionId equals p.QuestionId
                join e in enrolementRepository.Enrolement
                on p.ClassId equals e.ClassId
                where e.UserId == userId && e.Role == 2
                select q
            ).ToList();
        }
        public void UpdateQuestion(Question questionEntity)
        {
            this.Entry(questionEntity).State = EntityState.Modified;
            this.SaveChanges();
        }
        public void DeleteQuestion(Guid questionId)
        {
            var questionEntity = this.Questions.Find(questionId);
            if (questionEntity == null)
            {
                throw new InvalidOperationException("failed to delete Question");
            }
            this.Questions.Remove(questionEntity);
            this.SaveChanges();
        }
    }
}