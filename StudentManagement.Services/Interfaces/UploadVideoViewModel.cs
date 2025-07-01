using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagement.Core.Entities.Course_file;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace StudentManagement.Web.ViewModels
{
    public class UploadVideoViewModel
    {
        [Required(ErrorMessage = "العنوان مطلوب")]
        public string Title { get; set; }

        [Required(ErrorMessage = "حدد ملف الفيديو")]
        [DataType(DataType.Upload)]
        public IFormFile? VideoFile { get; set; }

        [Required(ErrorMessage = "اختر الكورس")]
        public int CourseId { get; set; }

        public IEnumerable<Course>? Courses { get; set; }
    }
}
