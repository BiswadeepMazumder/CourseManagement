using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;  // Make sure you include the namespace where CourseDTO is located
using System.Linq;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly StudentManagementDbContext _context;

        public CourseController(StudentManagementDbContext context)
        {
            _context = context;
        }

        // 1st endpoint: ShowAllCourses - Returns a list of all courses as CourseDTO
        [HttpGet("ShowAllCourses")]
        public ActionResult<IEnumerable<CourseDTO>> ShowAllCourses()
        {
            var courses = _context.Courses
                .Select(c => new CourseDTO(
                    c.CourseId,
                    c.CourseName,
                    c.Description,
                    c.MaxSeats,
                    c.CurrentSeats,
                    c.StartDate,
                    c.EndDate))
                .ToList();

            if (courses == null || !courses.Any())
            {
                return NotFound("No courses found.");
            }

            return Ok(courses);
        }

        // 2nd endpoint: ShowCourseByID - Returns course details as CourseDTO by Course ID
        [HttpGet("ShowCourseByID/{id}")]
        public ActionResult<CourseDTO> ShowCourseByID(int id)
        {
            var course = _context.Courses
                .Where(c => c.CourseId == id)
                .Select(c => new CourseDTO(
                    c.CourseId,
                    c.CourseName,
                    c.Description,
                    c.MaxSeats,
                    c.CurrentSeats,
                    c.StartDate,
                    c.EndDate))
                .FirstOrDefault();

            if (course == null)
            {
                return NotFound($"Course with ID {id} not found.");
            }

            return Ok(course);
        }

        // 3rd endpoint: CreateCourse - Allows for the creation of a new course using CourseDTO
        [HttpPost("CreateCourse")]
        public ActionResult<CourseDTO> CreateCourse([FromBody] CourseDTO courseDTO)
        {
            if (courseDTO == null)
            {
                return BadRequest("Invalid course data.");
            }

            var course = new Course
            {
                CourseId = courseDTO.CourseId,
                CourseName = courseDTO.CourseName,
                Description = courseDTO.Description,
                MaxSeats = courseDTO.MaxSeats,
                CurrentSeats = courseDTO.CurrentSeats,
                StartDate = courseDTO.StartDate,
                EndDate = courseDTO.EndDate
            };

            _context.Courses.Add(course);
            _context.SaveChanges();

            return Ok(courseDTO);
        }
    }
}
