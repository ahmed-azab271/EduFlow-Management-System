using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites
{
    public class Enrollment : BaseEntity
    {
        public DateTime EnrolmentDate { get; set; } = DateTime.Now;
        public string? Grade { get; set; }



        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
