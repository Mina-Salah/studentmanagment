
using StudentManagement.Core.Entities.Course_file;
using System;
using System.Collections.Generic;

namespace StudentManagement.Core.Entities
{
    public class Exam
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public DateTime ExamDate { get; set; }

        public virtual Course Course { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}
