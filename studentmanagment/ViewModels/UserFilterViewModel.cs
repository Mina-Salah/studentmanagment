using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagement.Core.Entities;
using System.Collections.Generic;

namespace StudentManagement.Web.ViewModels
{
    public class UserFilterViewModel
    {
        public string Search { get; set; }
        public string RoleFilter { get; set; }

        public List<SelectListItem> Roles { get; set; }

        public IEnumerable<User> Users { get; set; }

        // Counters
        public int TotalUsers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int DeletedCount { get; set; }
    }
}
