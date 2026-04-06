using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VgcCollege.Domain
{
    public class Exam
    {
        public int Id { get; set; }
        
        public int CourseId { get; set; }
        
        public string Title { get; set; }
        
        public DateTime Date { get; set; }
       
        public int MaxScore { get; set; }
        
        public bool ResultsReleased { get; set; }
         // Fundamental for the visibility rule

        public virtual Course Course { get; set; }
    }
}
