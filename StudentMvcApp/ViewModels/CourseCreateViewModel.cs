namespace StudentMvcApp.ViewModels
{
    public class CourseCreateViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> SubCourseIds { get; set; }
    }
}
