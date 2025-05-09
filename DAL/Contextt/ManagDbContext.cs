using Microsoft.EntityFrameworkCore;
using StudentManagement.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Data.Contextt
{
    public class ManagDbContext : DbContext
    {
        public ManagDbContext(DbContextOptions<ManagDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
/*        public DbSet<StudentSubject> StudentSubjects { get; set; }
*/        public DbSet<User> users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Subject>().HasQueryFilter(s => !s.IsDeleted);

            modelBuilder.Entity<StudentSubject>()
                .HasKey(ss => new { ss.StudentId, ss.SubjectId });
        }
    }
}
