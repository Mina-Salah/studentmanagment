using StudentManagement.Core.Entities.Course_file;
using System;

namespace StudentManagement.Core.Entities
{
    public class StudentSubject
    {
        public int Id { get; set; } 
        public int StudentId { get; set; }
        public int SubjectId { get; set; }

        public int? CourseId { get; set; }

        public virtual Student Student { get; set; }
        public virtual Subject Subject { get; set; }

        public virtual Course Course { get; set; } 
    }
}
