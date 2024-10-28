using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // Controller to manage student enrollments in courses
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly StudentManagementDbContext _context;

        // Constructor that injects the database context
        public EnrollmentsController(StudentManagementDbContext context)
        {
            _context = context;
        }

        // GET: api/Enrollments/GetAllEnrollments
        // Retrieves a list of all enrollments, including associated course and student details
        [HttpGet("GetAllEnrollments")]
        public async Task<IActionResult> GetAllEnrollments()
        {
            // Query to retrieve enrollment records, including course and student details
            var enrollments = await _context.Enrollments
                .Include(e => e.Course)  // Include related Course entity for course details
                .Include(e => e.Student) // Include related Student entity for student details
                .Select(e => new
                {
                    StudentName = e.Student != null ? e.Student.FirstName + " " + e.Student.LastName : "Unknown", // Full name of student
                    CourseName = e.Course != null ? e.Course.CourseName : "Unknown",  // Name of the course
                    CompletionStatus = e.CompletionStatus,  // Enrollment completion status
                    EnrollmentDate = e.EnrollmentDate.ToShortDateString() // Enrollment date (formatted to show only date)
                })
                .ToListAsync();

            // Check if any enrollments were found; if none, return a 404 Not Found response
            if (enrollments == null || !enrollments.Any())
            {
                return NotFound("No enrollments found.");
            }

            // Structure the response with headers and data
            var response = new
            {
                Headers = new[] { "StudentName", "CourseName", "CompletionStatus", "EnrollmentDate" },
                Data = enrollments
            };

            // Return the enrollment data in an HTTP 200 OK response
            return Ok(response);
        }

        // POST: api/Enrollments/JoinCourse
        // Endpoint to handle a student enrolling in a specific course
        [HttpPost("JoinCourse")]
        public async Task<IActionResult> JoinCourse([FromBody] EnrollmentRequestDTO enrollmentRequest)
        {
            // Validate the incoming request to ensure both student and course names are provided
            if (enrollmentRequest == null || string.IsNullOrEmpty(enrollmentRequest.StudentName) || string.IsNullOrEmpty(enrollmentRequest.CourseName))
            {
                return BadRequest("Invalid enrollment request. Please provide both student name and course name.");
            }

            // Attempt to find the student by their full name
            var student = await _context.Users
                .FirstOrDefaultAsync(u => u.FirstName + " " + u.LastName == enrollmentRequest.StudentName);

            // If the student is not found, return a 404 Not Found response
            if (student == null)
            {
                return NotFound($"Student '{enrollmentRequest.StudentName}' not found.");
            }

            // Check the number of active enrollments (CompletionStatus == "0") for the student
            var activeEnrollmentsCount = await _context.Enrollments
                .CountAsync(e => e.StudentId == student.UserId && e.CompletionStatus == "0");

            // Enforce a rule that limits students to 3 active classes at a time
            if (activeEnrollmentsCount >= 3)
            {
                return BadRequest($"Student '{enrollmentRequest.StudentName}' is already enrolled in 3 active classes. Complete a class before enrolling in a new one.");
            }

            // Attempt to find the course by its name
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseName == enrollmentRequest.CourseName);

            // If the course is not found, return a 404 Not Found response
            if (course == null)
            {
                return NotFound($"Course '{enrollmentRequest.CourseName}' not found.");
            }

            // Check if the student is already enrolled in the course to prevent duplicate enrollments
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == student.UserId && e.CourseId == course.CourseId);

            if (existingEnrollment != null)
            {
                return Conflict($"Student '{enrollmentRequest.StudentName}' is already enrolled in '{enrollmentRequest.CourseName}'.");
            }

            // Create a new enrollment record for the student
            var newEnrollment = new Enrollment
            {
                StudentId = student.UserId,
                CourseId = course.CourseId,
                EnrollmentDate = DateOnly.FromDateTime(DateTime.Now), // Store only the date portion
                CompletionStatus = "0"  // Default to "0" for incomplete status
            };

            // Add the new enrollment to the database and save changes
            _context.Enrollments.Add(newEnrollment);
            await _context.SaveChangesAsync();

            // Return a response indicating successful enrollment along with enrollment details
            return CreatedAtAction(nameof(GetAllEnrollments), new { id = newEnrollment.EnrollmentId }, new
            {
                Message = $"Student '{enrollmentRequest.StudentName}' has been enrolled in '{enrollmentRequest.CourseName}'.",
                EnrollmentId = newEnrollment.EnrollmentId,
                StudentName = enrollmentRequest.StudentName,
                CourseName = enrollmentRequest.CourseName,
                CompletionStatus = newEnrollment.CompletionStatus,
                EnrollmentDate = newEnrollment.EnrollmentDate.ToShortDateString() // Return only the date
            });
        }
    }

    // DTO class to represent enrollment requests for enrolling in a course
    public class EnrollmentRequestDTO
    {
        public string StudentName { get; set; } // Full name of the student
        public string CourseName { get; set; } // Name of the course
    }
}
