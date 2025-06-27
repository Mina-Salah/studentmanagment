
using System;
using System.Collections.Generic;

namespace StudentManagement.Core.Entities
{
    public class Assignment
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string Title { get; set; }
        public string Instructions { get; set; }
        public DateTime DueDate { get; set; }

        public virtual Lesson Lesson { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
