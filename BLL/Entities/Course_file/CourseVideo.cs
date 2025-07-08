namespace StudentManagement.Core.Entities.Course_file
{
    public class CourseVideo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string VideoPath { get; set; }
        public string TeacherEmail { get; set; }

        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        public int? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public bool IsDeleted { get; set; } = false;

        // ✅ المدة الإجمالية للفيديو بالدقائق
        public int DurationInMinutes { get; set; }
    }
}
