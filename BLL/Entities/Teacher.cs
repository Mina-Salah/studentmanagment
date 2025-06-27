using StudentManagement.Core.Entities.Course_file;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Core.Entities
{
    public class Teacher
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }

        public int? UserId { get; set; }  // الربط مع حساب المستخدم
        public User? User { get; set; }

        public ICollection<Course>? Courses { get; set; }
    }


}
