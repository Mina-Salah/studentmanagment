using System;
using System.Collections.Generic;

namespace StudentManagement.Core.Entities
{
    public class Quiz
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;   // Quiz title
        public int LessonId { get; set; }                   // Foreign key to Lesson
        public int TotalMarks { get; set; }                 // Total score
        public bool IsDeleted { get; set; } = false;        // Soft delete flag
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Lesson Lesson { get; set; }                  // Parent lesson
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<StudentQuiz> StudentQuizzes { get; set; } = new List<StudentQuiz>();
    }
}
