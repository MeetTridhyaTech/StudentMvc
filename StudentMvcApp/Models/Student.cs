using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StudentMvcApp.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> CourseIds { get; set; }
        public string? CourseNames { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
