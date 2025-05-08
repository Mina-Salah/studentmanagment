using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Web.ViewModels
{
    public class SubjectCheckboxItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Subject name is required.")]
        [StringLength(100, ErrorMessage = "Subject name cannot exceed 100 characters.")]
        [Display(Name = "Subject Name")]
        public string Name { get; set; }
        public bool IsSelected { get; set; }

    }
}
