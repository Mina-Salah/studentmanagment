using System;
using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Web.ViewModels
{
    public class TeacherViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        public string Name { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        [Display(Name = "تاريخ الميلاد")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

      /*  [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]*/
        public string? Password { get; set; }
    }
}
