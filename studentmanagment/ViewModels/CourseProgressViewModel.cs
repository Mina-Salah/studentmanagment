namespace StudentManagement.Web.ViewModels
{
    public class CourseProgressViewModel
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public int TotalVideos { get; set; }
        public int WatchedVideosCount { get; set; }
        public int TotalWatchedMinutes { get; set; }

        public int Progress => TotalVideos == 0 ? 0 : (int)Math.Round((double)WatchedVideosCount / TotalVideos * 100);
    }

}
