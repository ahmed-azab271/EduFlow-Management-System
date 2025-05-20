using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites
{
    public class Course : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int Credits { get; set; }



        public int TeacherId { get; set; }        
        public Teacher Teacher { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
        public ICollection<Assignment> Assignments { get; set; } = new HashSet<Assignment>();
    }
}
