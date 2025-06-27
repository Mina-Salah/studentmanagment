
namespace StudentManagement.Core.Entities
{
    public class ExamResult
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int StudentId { get; set; }
        public float Score { get; set; }

        public virtual Exam Exam { get; set; }
        public virtual User Student { get; set; }
    }
}
