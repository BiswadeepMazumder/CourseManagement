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

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly StudentManagementDbContext _context;

    public StudentsController(StudentManagementDbContext context)
    {
        _context = context;
    }

    [HttpGet("ShowAllStudents")]
    public ActionResult<IEnumerable<StudentDTO>> ShowAllStudents()
    {
        var students = _context.Users
            .Where(u => u.UserType == 2)  // Assuming 2 represents students
            .Select(u => new StudentDTO(u.FirstName, u.LastName, u.Email, u.EnrollmentDate,u.UserId))
            .ToList();

        if (students == null || !students.Any())
        {
            return NotFound("No students found.");
        }

        return Ok(students);
    }

    [HttpGet("ShowStudentByID/{id}")]
    public ActionResult<StudentDTO> ShowStudentByID(int id)
    {
        var student = _context.Users
            .Where(u => u.UserId == id && u.UserType == 2)  // Check for student with matching ID
            .Select(u => new StudentDTO(u.FirstName, u.LastName, u.Email, u.EnrollmentDate,u.UserId))
            .FirstOrDefault();

        if (student == null)
        {
            return NotFound($"Student with ID {id} not found.");
        }

        return Ok(student);
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginDTO loginDTO)
    {
        if (loginDTO == null || string.IsNullOrEmpty(loginDTO.Email) || string.IsNullOrEmpty(loginDTO.Password))
        {
            return BadRequest("Invalid login data.");
        }

        var user = _context.Users
            .FirstOrDefault(u => u.Email == loginDTO.Email && u.Password == loginDTO.Password);

        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        // In a real application, you would return a token or session information.
        //return Ok($"Welcome {user.FirstName} {user.LastName}, {user.UserType}");
        //return Ok(new { UserType = user.UserType });
        return Ok(new 
    { 
        StatusCode = 200, // Explicitly return status code
        FullName = $"{user.FirstName} {user.LastName}",  // Concatenate first name and last name
        UserType = user.UserType,  // Return the user type
        UserId = user.UserId       // Return the user ID
    });
    }
        
    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        // In a real scenario, you might invalidate a token or session here.
        return Ok();
    }
 }

}