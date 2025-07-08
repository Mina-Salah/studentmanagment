using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task EnrollStudentInCourseAsync(int studentId, int courseId);
        Task<bool> IsStudentEnrolledAsync(int studentId, int courseId);
        Task<List<int>> GetEnrolledCourseIdsForStudentAsync(int studentId);
        Task<int> GetEnrollmentCountAsync(int courseId); 
    }


}
