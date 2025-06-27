
namespace StudentManagement.Core.Entities.Course_file
{
    public class CourseVideo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string VideoPath { get; set; }
        public string TeacherEmail { get; set; }  // Not nullable in DB    }
    }
}
