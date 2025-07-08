using StudentManagement.Core.Entities.Course_file;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;


        // Navigation
        public ICollection<Course>? Courses { get; set; }
    }

}
