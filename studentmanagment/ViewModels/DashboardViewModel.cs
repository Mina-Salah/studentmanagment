namespace StudentManagement.Web.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int DeletedUsers { get; set; }

        public int TotalStudents { get; set; }
        public int ActiveStudents { get; set; }
        public int DeletedStudents { get; set; }

        public int TotalTeachers { get; set; }
        public int ActiveTeachers { get; set; }
        public int DeletedTeachers { get; set; }

        public int TotalCourses { get; set; }
        public int TotalSubjects { get; set; }
    }

}
