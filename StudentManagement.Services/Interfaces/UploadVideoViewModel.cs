using Microsoft.AspNetCore.Http;
using StudentManagement.Core.Entities.Course_file;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Web.ViewModels
{
    public class UploadVideoViewModel
    {
        [Required(ErrorMessage = "العنوان مطلوب")]
        public string Title { get; set; }

        [Required(ErrorMessage = "اختر الكورس")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "ملف الفيديو مطلوب")]
        public IFormFile VideoFile { get; set; }
    }
}
