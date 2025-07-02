using StudentManagement.Core.Entities.Course_file;
using StudentManagement.Core.Entities;

public class Teacher
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }            // ✅ مضافة حديثًا
    public DateTime? DateOfBirth { get; set; }      // ✅ مضافة حديثًا

    public int? UserId { get; set; }
    public User? User { get; set; }

    //public ICollection<Course>? Courses { get; set; }
    public ICollection<CourseVideo> CourseVideos { get; set; }
    public ICollection<CourseTeacher> CourseTeachers { get; set; } = new List<CourseTeacher>();

    public bool IsDeleted { get; set; } = false;
}
