using System.Text.Json.Serialization;

namespace StudentMvcApp.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentCourseId { get; set; }
        [JsonIgnore]
        public Course? ParentCourse { get; set; }
        public List<Course>? SubCourses { get; set; }
        public bool IsDeleted { get; set; } = false;
    }   
}
