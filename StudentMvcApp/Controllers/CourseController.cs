using Microsoft.AspNetCore.Mvc;
using StudentMvcApp.Data;
using StudentMvcApp.Models;
using System.Linq;

namespace StudentMvcApp.Controllers
{
    public class CourseController : Controller
    {
        private readonly StudentDbContext _context;

        public CourseController(StudentDbContext context) 
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var courses = _context.Courses.ToList();
            ViewBag.Today = new DateTime(2026, 1, 1);
            ViewBag.TitleMessage = "Course Management Panel";
            ViewData["Note"] = "You can add, edit and delete course below.";
            return View(courses);
        }

        [HttpGet]
        [Route("Course/GetAll")]
        public IActionResult GetAll()
        {  
            var courses = _context.Courses
                .Where(c => !c.IsDeleted && c.ParentCourseId== null)
                .Select(c => new
            {
                id = c.Id,
                name = c.Name,
                subCourse = _context.Courses
                    .Where(sc => sc.ParentCourseId == c.Id && !sc.IsDeleted)
                    .Select(sc => new { id = sc.Id, name = sc.Name })
                    .ToList() 
                }).ToList();
                
            return Json(courses);
        } 

        [HttpGet]
        [Route("Course/GetSubCourses/{parentId}")]
        public IActionResult GetSubCourses(int parentId)
        {
            var subCourses = _context.Courses
                .Where(sc => sc.ParentCourseId == parentId && !sc.IsDeleted)
                .Select(sc => new { id = sc.Id, name = sc.Name })
                .ToList(); 

            return Json(subCourses);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Route("Course/CreateJson")]
        public IActionResult CreateJson([FromBody] Course course)
        {
            if (string.IsNullOrWhiteSpace(course.Name))
            {
                ModelState.AddModelError("Name", "Course name is required.");
            }
            if (course.ParentCourseId != null)
            {
                var parentExists = _context.Courses.Any(c => c.Id == course.ParentCourseId && !c.IsDeleted);
                if (!parentExists)
                {
                    ModelState.AddModelError("ParentCourseId", "Selected parent course does not exist.");
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Courses.Add(course);
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                message = $"Course '{course.Name}' created successfully!",
                id = course.Id,
                name = course.Name
            });
        }


        [HttpGet]
        [Route("Course/EditView/{id}")]
        public IActionResult Edit(int id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == id);
            if(course== null)
            {
                return NotFound(new {message= "Course not found"});
            }
            return View(course);
        }
        [HttpPost]
        [Route("Course/EditJson")]
        public IActionResult EditJson([FromBody] Course course)
        {
            if(ModelState.IsValid)
            {
                var existingCourse = _context.Courses.FirstOrDefault(c => c.Id == course.Id);
                if(existingCourse == null)
                {
                    return NotFound(new { message = "Course not found" });
                }

                existingCourse.Name = course.Name;
                _context.SaveChanges();

                return Json(new { success = true, message = "Course updated successfully" });
            }
            return BadRequest(ModelState);
        }
        [HttpPost]
        [Route("Course/DeleteJson")]
        public IActionResult DeleteJson([FromBody] int id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == id);
            if(course== null)
            {
                return NotFound(new { message = "Course not found" });
            }

            course.IsDeleted = true; // Soft delete
            //_context.Courses.Remove(course);
            _context.SaveChanges();
            return Json(new { success = true, message = "Course deleted successfully " });
        }
    }
}
