using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites
{
    public class Assignment : BaseEntity
    {
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime PostedDate { get; set; } = DateTime.Now;



        public int CourseId { get; set; }
        public Course Course { get; set; }
        public ICollection<Submission> Submissions { get; set; } = new HashSet<Submission>();
    }
}
