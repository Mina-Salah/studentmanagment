using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Core.Entities
{
    public class Attachment
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }

        // يمكن ربطها بـ Course أو Lesson أو Assignment حسب مشروعك
        public int? LessonId { get; set; }
        public Lesson? Lesson { get; set; }
    }

}
