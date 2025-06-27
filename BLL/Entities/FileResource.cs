
using System;

namespace StudentManagement.Core.Entities
{
    public class FileResource
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }

        public virtual Lesson Lesson { get; set; }
    }
}
