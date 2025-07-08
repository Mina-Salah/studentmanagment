using StudentManagement.Core.Entities; 
namespace StudentManagement.Services.Interfaces  
{
    public interface IStudentService
    {
        // Asynchronous method for creating a new student with a list of associated subject IDs
        // تعديل الميثود CreateStudentAsync لتأخذ email و password أيضًا:
        Task CreateStudentAsync(Student student, List<int> subjectIds, string email, string password);

        Task<IEnumerable<Student>> GetAllStudentsAsync();

        Task<Student?> GetStudentByIdAsync(int id);

        Task UpdateStudentAsync(int id, Student student, List<int> subjectIds);

        Task DeleteStudentAsync(int id);

        Task<IEnumerable<Student>> GetDeletedStudentsAsync();

        Task RestoreStudentAsync(int id);

        Task DeleteStudentPermanentlyAsync(int id);
        Task<Student?> GetStudentByEmailAsync(string email);
   
    }
}
