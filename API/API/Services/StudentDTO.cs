using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class StudentDTO
    {
        public string FullName { get; set; }
        public int StudentId { get; set; }
        public string Email { get; set; }
        public DateOnly? EnrollmentDate { get; set; }

        // Constructor to initialize the values
        public StudentDTO(string firstName, string lastName, string email, DateOnly? enrollmentDate, int studendId)
        {
            FullName = $"{firstName} {lastName}";
            Email = email;
            EnrollmentDate = enrollmentDate;
            StudentId = studendId;
        }
    }
}