using System;

namespace SqlDemo.Models
{
    public class Assignment
    {
        public Guid AssignmentId { get; set; }
        public Guid ProblemId { get; set; }
    }
}