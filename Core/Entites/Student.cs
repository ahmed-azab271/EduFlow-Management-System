using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites
{
    public class Student : BaseEntity
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string Email { get; set; }
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        public string ApplicationUserId { get; set; }



        public ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
        public ICollection<Submission> Submissions { get; set; } = new HashSet<Submission>();
    }
}
