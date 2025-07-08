using StudentManagement.Core.Entities.Course_file;
using System.Collections.Generic;

namespace StudentManagement.Web.ViewModels
{
    public class VideoCourseViewModel
    {
        public int CourseId { get; set; }
        public List<CourseVideo> Videos { get; set; }
        public List<int> WatchedVideoIds { get; set; }
    }
}
