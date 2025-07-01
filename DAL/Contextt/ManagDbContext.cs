using Microsoft.EntityFrameworkCore;
using StudentManagement.Core.Entities;
using StudentManagement.Core.Entities.Course_file;

public class ManagDbContext : DbContext
{
    public ManagDbContext(DbContextOptions<ManagDbContext> options) : base(options) { }

    // === DbSets for All Entities ===
    public DbSet<User> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<CourseVideo> CourseVideos { get; set; }
    public DbSet<VideoAccess> VideoAccesses { get; set; }

    public DbSet<StudentSubject> StudentSubjects { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<StudentQuiz> StudentQuizzes { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Certificate> Certificates { get; set; }
    public DbSet<Discussion> Discussions { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<ExamResult> ExamResults { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<FileResource> FileResources { get; set; }
    public DbSet<Submission> Submissions { get; set; }

    // Newly added entities
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Teacher> Teachers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // === Global Soft Delete Filters ===
        modelBuilder.Entity<Subject>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<Course>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Lesson>().HasQueryFilter(l => !l.IsDeleted);
        modelBuilder.Entity<Question>().HasQueryFilter(q => !q.IsDeleted);
        modelBuilder.Entity<Quiz>().HasQueryFilter(q => !q.IsDeleted);

        // === StudentSubject Relations ===
        modelBuilder.Entity<StudentSubject>()
            .HasKey(ss => ss.Id);

        modelBuilder.Entity<StudentSubject>()
            .HasOne(ss => ss.Student)
            .WithMany(s => s.StudentSubjects)
            .HasForeignKey(ss => ss.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StudentSubject>()
            .HasOne(ss => ss.Subject)
            .WithMany()
            .HasForeignKey(ss => ss.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentSubject>()
            .HasOne(ss => ss.Course)
            .WithMany()
            .HasForeignKey(ss => ss.CourseId)
            .OnDelete(DeleteBehavior.SetNull);

        // === StudentQuiz Relations ===
        modelBuilder.Entity<StudentQuiz>()
            .HasKey(sq => sq.Id);

        modelBuilder.Entity<StudentQuiz>()
            .HasOne(sq => sq.Quiz)
            .WithMany(q => q.StudentQuizzes)
            .HasForeignKey(sq => sq.QuizId);

        // ✅ ✅ ✅ هذه العلاقات هي الأهم 👇

        // علاقة 1-1: الطالب يملك مستخدم
        modelBuilder.Entity<Student>()
            .HasOne(s => s.User)
            .WithOne(u => u.Student)
            .HasForeignKey<Student>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // علاقة 1-1: المدرس يملك مستخدم
        modelBuilder.Entity<Teacher>()
            .HasOne(t => t.User)
            .WithOne(u => u.Teacher)
            .HasForeignKey<Teacher>(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // === Quiz - Lesson Relation ===
        modelBuilder.Entity<Quiz>()
            .HasOne(q => q.Lesson)
            .WithMany(l => l.Quizzes)
            .HasForeignKey(q => q.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        // === Answer - Question Relation ===
        modelBuilder.Entity<Answer>()
            .HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // === Course - Teacher Relation ===
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Teacher)
            .WithMany(t => t.Courses)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.SetNull);

        // === Course - Category Relation ===
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Category)
            .WithMany(cat => cat.Courses)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // === Attachment - Lesson Relation ===
        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.Lesson)
            .WithMany(l => l.Attachments)
            .HasForeignKey(a => a.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<VideoAccess>()
            .HasOne(va => va.Student)
            .WithMany()
            .HasForeignKey(va => va.StudentId);
        // teacher and course video
        modelBuilder.Entity<CourseVideo>()
        .HasOne(cv => cv.Teacher)
        .WithMany(t => t.CourseVideos)
        .HasForeignKey(cv => cv.TeacherId)
        .OnDelete(DeleteBehavior.Restrict); // أو حسب ما يناسبك


    }
}
