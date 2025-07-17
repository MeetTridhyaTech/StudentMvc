using Microsoft.AspNetCore.Mvc;
using StudentMvcApp.Models;
using StudentMvcApp.Data;

namespace StudentMvcApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentApiController : ControllerBase
    {
        private readonly StudentDbContext _context;

        public StudentApiController(StudentDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddStudent([FromBody] Student student)
        {
            if (student == null || string.IsNullOrWhiteSpace(student.Name))
                return BadRequest("Invalid student data.");

            _context.Students.Add(student);
            _context.SaveChanges();

            return Ok(student); // return saved record with Id
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, [FromBody] Student updatedStudent)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == id);
            if (student == null)
                return NotFound($"Student with Id {id} not found.");

            student.Name = updatedStudent.Name;
            student.CourseNames = updatedStudent.CourseNames;

            _context.SaveChanges();

            return Ok(student);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == id);
            if (student == null)
                return NotFound($"Student with Id {id} not found.");

            _context.Students.Remove(student);
            _context.SaveChanges();

            return Ok($"Student with Id {id} deleted successfully.");
        }
    }
}
