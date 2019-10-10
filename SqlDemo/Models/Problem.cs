using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlDemo.Models
{
    public class Problem
    {
        public Guid ProblemId { get; set; }
        public Guid ClassId { get; set; }
        public Guid StudentId { get; set; }
        public Guid QuestionId { get; set; }
        public string StudentResponse { get; set; }
        public int State { get; set; }
        public string TeacherResponse { get; set; }
    }
}