using System;
using System.Collections.Generic;
using SqlDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace SqlDemo.Models
{
    public interface IQuestionRepository
    {
        DbSet<Question> Questions { get; set; }
        void CreateQuestion(Question questionEntity);
        IEnumerable<Question> GetQuestions();
        Question GetQuestion(Guid QuestionId);
        IEnumerable<Question> GetQuestions(string question);
        IEnumerable<Question> FindQuestionsByClass(Guid classId, IProblemRepository problemRepository);
        IEnumerable<Question> FindQuestionsByStudent(Guid userId, IProblemRepository problemRepository);
        IEnumerable<Question> FindQuestionsByTeacher(Guid userId, IProblemRepository problemRepository, IEnrolementRepository enrolementRepository);
        void UpdateQuestion(Question questionEntity);
        void DeleteQuestion(Guid questionId);
    }
}