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
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly StudentManagementDbContext _context;

        public EnrollmentsController(StudentManagementDbContext context)
        {
            _context = context;
        }

        // GET: api/Enrollments/GetAllEnrollments
        [HttpGet("GetAllEnrollments")]
        public async Task<IActionResult> GetAllEnrollments()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Course)  // Include the related Course entity
                .Include(e => e.Student) // Include the related Student entity (User)
                .Select(e => new
                {
                    StudentName = e.Student != null ? e.Student.FirstName + " " + e.Student.LastName : "Unknown",  // Get the student's name
                    CourseName = e.Course != null ? e.Course.CourseName : "Unknown",  // Get the course's name
                    CompletionStatus = e.CompletionStatus,  // Return the completion status
                    EnrollmentDate = e.EnrollmentDate.ToShortDateString() // Format the EnrollmentDate to only show the date
                })
                .ToListAsync();

            if (enrollments == null || !enrollments.Any())
            {
                return NotFound("No enrollments found.");
            }

            var response = new
            {
                Headers = new[] { "StudentName", "CourseName", "CompletionStatus", "EnrollmentDate" },
                Data = enrollments
            };

            return Ok(response);
        }

        // POST: api/Enrollments/JoinCourse
        [HttpPost("JoinCourse")]
public async Task<IActionResult> JoinCourse([FromBody] EnrollmentRequestDTO enrollmentRequest)
{
    if (enrollmentRequest == null || string.IsNullOrEmpty(enrollmentRequest.StudentName) || string.IsNullOrEmpty(enrollmentRequest.CourseName))
    {
        return BadRequest("Invalid enrollment request. Please provide both student name and course name.");
    }

    // Find the student by name
    var student = await _context.Users
        .FirstOrDefaultAsync(u => u.FirstName + " " + u.LastName == enrollmentRequest.StudentName);

    if (student == null)
    {
        return NotFound($"Student '{enrollmentRequest.StudentName}' not found.");
    }

    // Check how many active enrollments (with CompletionStatus == "0") the student has
    var activeEnrollmentsCount = await _context.Enrollments
        .CountAsync(e => e.StudentId == student.UserId && e.CompletionStatus == "0");

    if (activeEnrollmentsCount >= 3)
    {
        return BadRequest($"Student '{enrollmentRequest.StudentName}' is already enrolled in 3 active classes. You cannot enroll in more classes until one is completed.");
    }

    // Find the course by name
    var course = await _context.Courses
        .FirstOrDefaultAsync(c => c.CourseName == enrollmentRequest.CourseName);

    if (course == null)
    {
        return NotFound($"Course '{enrollmentRequest.CourseName}' not found.");
    }

    // Check if the student is already enrolled in this course
    var existingEnrollment = await _context.Enrollments
        .FirstOrDefaultAsync(e => e.StudentId == student.UserId && e.CourseId == course.CourseId);

    if (existingEnrollment != null)
    {
        return Conflict($"Student '{enrollmentRequest.StudentName}' is already enrolled in '{enrollmentRequest.CourseName}'.");
    }

    // Create a new enrollment with DateOnly for EnrollmentDate
    var newEnrollment = new Enrollment
    {
        StudentId = student.UserId,
        CourseId = course.CourseId,
        EnrollmentDate = DateOnly.FromDateTime(DateTime.Now),  // Store only the date
        CompletionStatus = "0"  // Default to 0 (incomplete)
    };

    _context.Enrollments.Add(newEnrollment);
    await _context.SaveChangesAsync();

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

    // DTO for enrollment request
    public class EnrollmentRequestDTO
    {
        public string StudentName { get; set; }
        public string CourseName { get; set; }
    }
}
