using Microsoft.AspNetCore.Http;
using StudentManagement.Core.Entities;
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
    }
}
