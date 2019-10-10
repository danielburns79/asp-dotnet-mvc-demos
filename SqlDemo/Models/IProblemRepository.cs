using System;
using System.Collections.Generic;
using SqlDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace SqlDemo.Models
{
    public interface IProblemRepository
    {
        DbSet<Problem> Problems { get; set; }
        void CreateProblem(Problem questionEntity);
        IEnumerable<Problem> GetProblems();
        Problem GetProblem(Guid ProblemId);
        IEnumerable<Problem> FindProblemsByQuestion(Guid questionId);
        IEnumerable<Problem> FindProblemsByClass(Guid classId);
        IEnumerable<Problem> FindProblemsByStudent(Guid userId);
        IEnumerable<Problem> FindProblemsByTeacher(Guid userId, IEnrolementRepository enrolementRepository);
        IEnumerable<Problem> FindProblemsByAssignment(Guid assignmentId, IAssignmentRepository assignmentRepository);
        void UpdateProblem(Problem questionEntity);
        void DeleteProblem(Guid questionId);
    }
}