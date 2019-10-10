using System;
using System.Collections.Generic;
using SqlDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace SqlDemo.Models
{
    public interface IAssignmentRepository
    {
        DbSet<Assignment> Assignments { get; set; }
        void CreateAssignment(Assignment assignmentEntity);
        IEnumerable<Assignment> GetAssignments();
        Assignment GetAssignment(Guid AssignmentId);
        IEnumerable<Assignment> FindAssignmentsByClass(Guid classId, IProblemRepository problemRepository);
        IEnumerable<Assignment> FindAssignmentsByStudent(Guid userId, IProblemRepository problemRepository);
        IEnumerable<Assignment> FindAssignmentsByTeacher(Guid userId, IProblemRepository problemRepository, IEnrolementRepository enrolementRepository);
        void UpdateAssignment(Assignment assignmentEntity);
        void DeleteAssignment(Guid AssignmentId);
    }
}