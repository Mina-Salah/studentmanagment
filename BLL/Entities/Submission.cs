
using System;

namespace StudentManagement.Core.Entities
{
    public class Submission
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string FilePath { get; set; }
        public string Feedback { get; set; }
        public float? Grade { get; set; }

        public virtual Assignment Assignment { get; set; }
        public virtual User Student { get; set; }
    }
}
