using System.IO;

namespace StudentManagement.Web.Helpers
{
   
        public static class FileHelper
        {
            public static void CreateCourseFolder(string basePath, string courseTitle)
            {
                var validName = string.Concat(courseTitle.Split(Path.GetInvalidFileNameChars()));
                var fullPath = Path.Combine(basePath, validName);

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                // للتجريب فقط:
                Console.WriteLine("📁 Folder created at: " + fullPath);
            }
        }
}

