
using StudentManagement.Core.Entities.Course_file;
using System;

namespace StudentManagement.Core.Entities
{
    public class Certificate
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public DateTime IssuedAt { get; set; }
        public string CertificateUrl { get; set; }

        public virtual Course Course { get; set; }
        public virtual User Student { get; set; }
    }
}
