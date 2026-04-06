using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VgcCollege.Domain
{
    public class CourseEnrolment
    {
        public int Id { get; set; }
        
        public int StudentProfileId { get; set; }
        
        public int CourseId { get; set; }
        
        public DateTime EnrolDate { get; set; }
        
        public string Status { get; set; }
        public int? Grade { get; set; }
        
        public bool IsReleased { get; set; }



        // Relationships
        public virtual StudentProfile Student { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; }
    }
}
