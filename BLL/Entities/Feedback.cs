
using StudentManagement.Core.Entities.Course_file;
using System;

namespace StudentManagement.Core.Entities
{
    public class Feedback
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Course Course { get; set; }
        public virtual User Student { get; set; }
    }
}
