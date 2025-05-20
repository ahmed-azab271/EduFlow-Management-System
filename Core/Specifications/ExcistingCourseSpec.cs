using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class ExcistingCourseSpec : Specification<Course>
    {
        public ExcistingCourseSpec() :base()
        {
            Include.Add(I => I.Teacher);
        }
        public ExcistingCourseSpec(string code) : base(N=>N.Code == code)
        {
            Include.Add(I => I.Teacher);
        }
        public ExcistingCourseSpec(string courseName , string? empty) : base(N => N.Name == courseName)
        {
            Include.Add(I => I.Teacher);
        }
    }
}
