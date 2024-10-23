using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int MaxSeats { get; set; }

    public int CurrentSeats { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int? AdminId { get; set; }

    public virtual User? Admin { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
