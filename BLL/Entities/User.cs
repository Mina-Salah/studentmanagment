
using System.Collections.Generic;

namespace StudentManagement.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
