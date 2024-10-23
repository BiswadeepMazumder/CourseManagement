using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class CourseDTO
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public int MaxSeats { get; set; }
        public int CurrentSeats { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        
        // Constructor to initialize the values
        public CourseDTO(int courseId, string courseName, string description, int maxSeats, int currentSeats, DateOnly startDate, DateOnly endDate)
        {
            CourseId = courseId;
            CourseName = courseName;
            Description = description;
            MaxSeats = maxSeats;
            CurrentSeats = currentSeats;
            StartDate = startDate;
            EndDate = endDate;
        }
    } 
}
