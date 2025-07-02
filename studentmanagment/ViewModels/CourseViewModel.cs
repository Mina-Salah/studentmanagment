using StudentManagement.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Web.ViewModels
{
    public class CourseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان الكورس مطلوب")]
        public string Title { get; set; }

        public string Description { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "تاريخ البداية مطلوب")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "تاريخ النهاية مطلوب")]
        public DateTime EndDate { get; set; }

       // [Required(ErrorMessage = "اختر المدرس")]
        //public int? TeacherId { get; set; }

        [Required(ErrorMessage = "اختر التصنيف")]
        public int? CategoryId { get; set; }


        [Required(ErrorMessage = "اختر المدرسين")]
        public List<int> TeacherIds { get; set; } = new();
        // للعرض فقط
        public string? TeacherName { get; set; }
        public string? CategoryName { get; set; }
        public List<int> SelectedTeacherIds { get; set; } = new(); // ⬅️ جديدة
        public string? TeacherNames { get; set; }

        public IEnumerable<Teacher> Teachers { get; set; } = new List<Teacher>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    }
}
