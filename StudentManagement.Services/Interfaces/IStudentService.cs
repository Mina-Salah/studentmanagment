using StudentManagement.Core.Entities; 
namespace StudentManagement.Services.Interfaces  
{
    public interface IStudentService
    {
        // Asynchronous method for creating a new student with a list of associated subject IDs
        Task CreateStudentAsync(Student student, List<int> subjectIds);

        // Asynchronous method to retrieve all students who are not marked as deleted
        Task<IEnumerable<Student>> GetAllStudentsAsync();

        // Asynchronous method to get a specific student by their ID
        Task<Student?> GetStudentByIdAsync(int id);

        // Asynchronous method to update an existing student's data and associated subjects
        Task UpdateStudentAsync(int id, Student student, List<int> subjectIds);

        // Asynchronous method to delete a student by marking them as deleted (soft delete)
        Task DeleteStudentAsync(int id);

        // Asynchronous method to retrieve all students who are marked as deleted
        Task<IEnumerable<Student>> GetDeletedStudentsAsync();

        // Asynchronous method to restore a deleted student by marking them as not deleted
        Task RestoreStudentAsync(int id);
    }
}
