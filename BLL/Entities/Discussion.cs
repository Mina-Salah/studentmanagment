
using System;

namespace StudentManagement.Core.Entities
{
    public class Discussion
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public int UserId { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Lesson Lesson { get; set; }
        public virtual User User { get; set; }
    }
}
