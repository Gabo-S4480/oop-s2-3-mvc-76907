using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VgcCollege.Domain
{
    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int BranchId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        // Foreign Key for Faculty
        // We keep the '?' here because int is a Value Type and needs it to be optional
        public int? FacultyProfileId { get; set; }

        // Navigation Property
        // We remove the '?' here to avoid the CS8632 warning in your current context
        public virtual FacultyProfile FacultyProfile { get; set; }

        // Relationships
        public virtual Branch Branch { get; set; }

        public virtual ICollection<CourseEnrolment> Enrolments { get; set; }

        public virtual ICollection<Assignment> Assignments { get; set; }

        public virtual ICollection<Exam> Exams { get; set; }

        public string ScheduleDay { get; set; } // Example: "Monday & Wednesday"

        public string ScheduleTime { get; set; }
    }
}