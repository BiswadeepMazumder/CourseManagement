using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;  // Importing the namespace where CourseDTO is defined
using System.Linq;
using System.Collections.Generic;

namespace API.Controllers
{
    // Defines the route for this controller as "api/Course" and enables API controller features such as automatic model validation
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        // Private field to hold the database context, allowing interaction with the database
        private readonly StudentManagementDbContext _context;

        // Constructor injection for database context, allowing dependency injection for testing and modularity
        public CourseController(StudentManagementDbContext context)
        {
            _context = context;
        }

        // 1st Endpoint: GET api/Course/ShowAllCourses
        // Retrieves all courses in the database and returns them as a list of CourseDTO objects
        [HttpGet("ShowAllCourses")]
        public ActionResult<IEnumerable<CourseDTO>> ShowAllCourses()
        {
            // Selects all courses and maps them to CourseDTO, transforming database models into simplified DTOs
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

            // Checks if no courses are found, returning a 404 Not Found response with an error message
            if (courses == null || !courses.Any())
            {
                return NotFound("No courses found.");
            }

            // Returns the list of courses with a 200 OK status
            return Ok(courses);
        }

        // 2nd Endpoint: GET api/Course/ShowCourseByID/{id}
        // Retrieves the course details for a specific course ID and returns the details as a CourseDTO object
        [HttpGet("ShowCourseByID/{id}")]
        public ActionResult<CourseDTO> ShowCourseByID(int id)
        {
            // Finds the course with the specified ID and maps it to a CourseDTO
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

            // Checks if the course was not found, returning a 404 Not Found response with a descriptive message
            if (course == null)
            {
                return NotFound($"Course with ID {id} not found.");
            }

            // Returns the found course with a 200 OK status
            return Ok(course);
        }

        // 3rd Endpoint: POST api/Course/CreateCourse
        // Creates a new course record in the database based on the provided CourseDTO data
        [HttpPost("CreateCourse")]
        public ActionResult<CourseDTO> CreateCourse([FromBody] CourseDTO courseDTO)
        {
            // Checks if the incoming courseDTO data is null, returning a 400 Bad Request if invalid data is detected
            if (courseDTO == null)
            {
                return BadRequest("Invalid course data.");
            }

            // Maps the CourseDTO data to the Course entity model for saving to the database
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

            // Adds the new course entity to the database and saves the changes
            _context.Courses.Add(course);
            _context.SaveChanges();

            // Returns the created CourseDTO with a 200 OK status
            return Ok(courseDTO);
        }
    }
}
