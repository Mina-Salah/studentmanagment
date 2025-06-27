using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Web.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter Name")]
        public string Name { get; set; }
    }
}
