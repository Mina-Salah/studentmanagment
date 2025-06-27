using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Core.Entities
{
    public class StudentQuiz
    {
        public int Id { get; set; }

        // Foreign Keys
        public int StudentId { get; set; }
        public int QuizId { get; set; }

        // Navigation properties
        public Student Student { get; set; }
        public Quiz Quiz { get; set; }

        // Extra info
        public double Score { get; set; }             // student score
        public DateTime TakenAt { get; set; }         // when student took the quiz
        public bool IsPassed { get; set; }            // pass/fail flag
    }
}
