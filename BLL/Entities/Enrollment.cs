
using StudentManagement.Core.Entities.Course_file;
using System;

namespace StudentManagement.Core.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
    }

}
