using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Core.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } // This is Email

        [Required]
        public string PasswordHash { get; set; }

        public string Role { get; set; } = "User";
    }
}
