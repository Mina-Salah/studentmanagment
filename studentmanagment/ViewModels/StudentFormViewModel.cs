using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Web.ViewModels
{
    public class StudentFormViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;

        // For subject checkboxes
        public List<SubjectCheckboxItem> Subjects { get; set; }
    }
}
