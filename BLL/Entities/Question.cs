
namespace StudentManagement.Core.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string Text { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectOption { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Answer>? Answers { get; set; }

        public virtual Exam Exam { get; set; }
    }
}
