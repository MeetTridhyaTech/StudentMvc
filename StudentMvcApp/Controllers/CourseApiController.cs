using Microsoft.AspNetCore.Mvc;
using StudentMvcApp.Data;
using StudentMvcApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace StudentMvcApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseApiController : ControllerBase
    {
        private readonly StudentDbContext _context;

        public CourseApiController(StudentDbContext context)
        {
            _context = context;
        }

        // GET: api/courseapi
        [HttpGet]
        public ActionResult<IEnumerable<Course>> GetCourses()
        {
            return Ok(_context.Courses.ToList());
        }

        // GET: api/courseapi/5
        [HttpGet("{id}")]
        public ActionResult<Course> GetCourse(int id)
        {
            var course = _context.Courses.Find(id);
            if (course == null)
                return NotFound();

            return Ok(course);
        }

        // POST: api/courseapi
        [HttpPost]
        public ActionResult<Course> CreateCourse([FromBody] Course course)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Courses.Add(course);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }

        // PUT: api/courseapi/5
        [HttpPut("{id}")]
        public IActionResult UpdateCourse(int id, [FromBody] Course updatedCourse)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
                return NotFound();

            course.Name = updatedCourse.Name;
            _context.SaveChanges();

            return Ok(course);
        }

        // DELETE: api/courseapi/5
        [HttpDelete("{id}")]
        public IActionResult DeleteCourse(int id)
        {
            var course = _context.Courses.Find(id);
            if (course == null)
                return NotFound();

            _context.Courses.Remove(course);
            _context.SaveChanges();

            return Ok($"Course with ID {id} deleted successfully.");
        }
    }
}
