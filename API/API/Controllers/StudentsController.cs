using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace API.Controllers
{
    // Controller to manage student-related actions, such as viewing student information and logging in
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentManagementDbContext _context;

        // Constructor to inject the StudentManagementDbContext for database operations
        public StudentsController(StudentManagementDbContext context)
        {
            _context = context;
        }

        // Endpoint to retrieve a list of all students
        // GET: api/Students/ShowAllStudents
        [HttpGet("ShowAllStudents")]
        public ActionResult<IEnumerable<StudentDTO>> ShowAllStudents()
        {
            // Query Users table for users with UserType == 2 (assuming 2 represents students)
            var students = _context.Users
                .Where(u => u.UserType == 2)
                .Select(u => new StudentDTO(u.FirstName, u.LastName, u.Email, u.EnrollmentDate, u.UserId))
                .ToList();

            // Check if no students are found, returning a 404 Not Found response if the list is empty
            if (students == null || !students.Any())
            {
                return NotFound("No students found.");
            }

            // Return the list of students in an HTTP 200 OK response
            return Ok(students);
        }

        // Endpoint to retrieve a specific student by ID
        // GET: api/Students/ShowStudentByID/{id}
        [HttpGet("ShowStudentByID/{id}")]
        public ActionResult<StudentDTO> ShowStudentByID(int id)
        {
            // Query for a specific student by UserId and UserType (2 for student)
            var student = _context.Users
                .Where(u => u.UserId == id && u.UserType == 2)
                .Select(u => new StudentDTO(u.FirstName, u.LastName, u.Email, u.EnrollmentDate, u.UserId))
                .FirstOrDefault();

            // Return a 404 Not Found response if the student is not found
            if (student == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            // Return the student details in an HTTP 200 OK response
            return Ok(student);
        }

        // Endpoint to handle student login
        // POST: api/Students/Login
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
            // Check if login data is missing or incomplete, returning a 400 Bad Request response if so
            if (loginDTO == null || string.IsNullOrEmpty(loginDTO.Email) || string.IsNullOrEmpty(loginDTO.Password))
            {
                return BadRequest("Invalid login data.");
            }

            // Attempt to find a user with the provided email and password
            var user = _context.Users
                .FirstOrDefault(u => u.Email == loginDTO.Email && u.Password == loginDTO.Password);

            // If the user is not found, return a 401 Unauthorized response indicating invalid credentials
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Return a success response with user details (in a real app, return a token or session)
            return Ok(new 
            { 
                StatusCode = 200,                         // Explicitly setting a success status code
                FullName = $"{user.FirstName} {user.LastName}", // Full name of the user
                UserType = user.UserType,                 // Type of user (e.g., student, admin)
                UserId = user.UserId                      // Unique user ID
            });
        }

        // Endpoint to handle student logout
        // POST: api/Students/Logout
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            // Here, token invalidation or session cleanup would occur in a real application
            return Ok("Logout successful.");
        }
    }
}
