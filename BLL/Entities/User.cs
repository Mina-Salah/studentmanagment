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

        // العلاقات مع الطالب والمدرس (يتم الربط من الطرف الآخر)
        public virtual Student Student { get; set; }
        public virtual Teacher Teacher { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
