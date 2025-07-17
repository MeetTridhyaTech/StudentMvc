using Microsoft.AspNetCore.Mvc;
using StudentMvcApp.Data;
using StudentMvcApp.Models;
using System.Linq;

namespace StudentMvcApp.Controllers
{
    public class StudentController : Controller
    {
        private readonly StudentDbContext _context;

        public StudentController(StudentDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            //ViewBag.Today = DateTime.Now;
            var students = _context.Students.ToList();
            ViewBag.Today = new DateTime(2025, 1, 1);
            ViewBag.TitleMessage = "Student Management System";
            ViewData["Note"] = "You can add, edit and delete course below.";
            return View(students);
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            var courseDict = _context.Courses.ToDictionary(c => c.Id, c => c.Name);

            var students = _context.Students
                .Where(s=> !s.IsDeleted)
                .AsEnumerable() // switch to client-side to avoid EF translation error
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.CourseIds,
                    CourseNames = s.CourseIds != null && s.CourseIds.Any()
                        ? string.Join(", ", s.CourseIds.Where(id => courseDict.ContainsKey(id)).Select(id => courseDict[id]))
                        : ""
                }).ToList();

            return Json(students);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Route("Student/CreateJson")]
        public IActionResult CreateJson([FromBody] Student student)
        {
            if(ModelState.IsValid)
            {
                student.CourseNames = string.Join(", ",
                    _context.Courses
                        .Where(c => student.CourseIds.Contains(c.Id))
                        .Select(c => c.Name));

                _context.Students.Add(student);
                _context.SaveChanges();
                TempData["Success"] = "Student Added Successfully";
                return Json(new { success = true, message = "Student added successfully" });
            }

            return BadRequest(ModelState);
        }
        public IActionResult Edit(int id)
        {
            var student = _context.Students.FirstOrDefault(student => student.Id == id);
            if (student == null)
                return NotFound();

            return View(student);
        }
        [HttpPost]
        [Route("Student/EditJson")]
        public IActionResult Edit([FromBody] Student student)
        {
            if (ModelState.IsValid)
            {
                var existingStudent = _context.Students.FirstOrDefault(s => s.Id == student.Id);
                if (existingStudent == null)
                    return NotFound(new { message = "Student not found" });

                existingStudent.Name = student.Name;
                existingStudent.CourseNames = string.Join(", ",
                    _context.Courses
                            .Where(c => student.CourseIds.Contains(c.Id))
                            .Select(c => c.Name));

                _context.SaveChanges();
                return Json(new { success = true, message = "Student updated successfully" });
            }

            return BadRequest(ModelState);
        }


        public IActionResult Delete(int id)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == id);
            if (student == null)
                return NotFound();

            return View(student);
        }

        [HttpPost]
        //, ActionName("Delete")]
        [Route("Student/DeleteJson")]
        public IActionResult DeleteConfirmed(int id)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == id);
            if (student != null)
            {
                student.IsDeleted = true;
                //_context.Students.Remove(student);
                _context.SaveChanges();
                return Json(new { success = true, message = "Student deleted successfully" });

            }
            return NotFound(new { success = false, message = "Student not found" });
        }

    }
}
