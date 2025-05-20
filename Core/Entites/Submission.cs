using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites
{
    public class Submission : BaseEntity
    {
        public string FileUrl { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
        public string? Grade { get; set; }
        public string? FeedBack { get; set; }



        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
