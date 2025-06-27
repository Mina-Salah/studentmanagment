namespace StudentManagement.Core.Entities.Course_file
{
    public class VideoAccess
    {
        public int Id { get; set; }

        public int CourseVideoId { get; set; }
        public CourseVideo CourseVideo { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
