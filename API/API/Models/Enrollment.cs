using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Enrollment
{
     public int EnrollmentId { get; set; }

    public int? StudentId { get; set; }

    public int? CourseId { get; set; }

    public DateOnly EnrollmentDate { get; set; }

    public string CompletionStatus { get; set; } = null!;

    public virtual Course? Course { get; set; }  // Navigation property for Course

    public virtual User? Student { get; set; }   // Navigation property for Student

}
