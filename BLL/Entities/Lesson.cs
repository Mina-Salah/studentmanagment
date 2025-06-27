
using StudentManagement.Core.Entities.Course_file;

namespace StudentManagement.Core.Entities
{
    public class Lesson
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ResourceUrl { get; set; }
        public ICollection<Quiz> Quizzes { get; set; }
        public virtual Course Course { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Attachment>? Attachments { get; set; }

    }
}
