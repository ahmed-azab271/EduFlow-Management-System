using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites
{
    public class Teacher :BaseEntity
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string Email { get; set; }
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        public string ApplicationUserId { get; set; }



        public ICollection<Course> Courses { get; set; } = new HashSet<Course>();
    }
}
