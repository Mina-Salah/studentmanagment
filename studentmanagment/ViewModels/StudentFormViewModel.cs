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
       
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string ?Password { get; set; }
        // For subject checkboxes
        public List<SubjectCheckboxItem> Subjects { get; set; }
    }
}
