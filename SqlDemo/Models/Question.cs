using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlDemo.Models
{
    public class Question
    {
        public Guid QuestionId { get; set; }
        [Column("Question")]
        public string QuestionString { get; set; }
        public string DesiredResponse { get; set; }
    }
}