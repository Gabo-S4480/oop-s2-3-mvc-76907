using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VgcCollege.Domain
{
    public class ExamResult
    {
        public int Id { get; set; }
        
        public int ExamId { get; set; }
        
        public int StudentProfileId { get; set; }
       
        public double Score { get; set; }
       
        public string Grade { get; set; }
       
    }
}
