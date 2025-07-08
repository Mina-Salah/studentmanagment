using StudentManagement.Core.Entities;

namespace StudentManagement.Web.ViewModels
{
    public class StudentDashboardViewModel
    {
        public Student StudentName { get; set; }
        public List<CourseProgressViewModel> Courses { get; set; }
    }
}
