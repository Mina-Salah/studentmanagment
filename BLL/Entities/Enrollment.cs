
using StudentManagement.Core.Entities.Course_file;
using System;

namespace StudentManagement.Core.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public DateTime EnrolledAt { get; set; }

        public virtual Course Course { get; set; }
        public virtual User Student { get; set; }
    }
}
