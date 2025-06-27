using System;
using System.Collections.Generic;

namespace StudentManagement.Core.Entities.Course_file
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Teacher relation
        public int? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        // Category relation
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public bool IsDeleted { get; set; }

        // Navigation Collections
        public virtual ICollection<Enrollment>? Enrollments { get; set; }
        public virtual ICollection<Lesson>? Lessons { get; set; }
        public virtual ICollection<Exam>? Exams { get; set; }
    }
}
