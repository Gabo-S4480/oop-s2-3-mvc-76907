using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VgcCollege.Domain
{
    public class Branch
    {
        public int Id { get; set; }
       
        public string Name { get; set; }
        
        public string Address { get; set; }
        

        // Relationship
        public virtual ICollection<Course> Courses { get; set; }
    }
}
