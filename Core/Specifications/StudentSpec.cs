using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class StudentSpec : Specification<Student>
    {
        public StudentSpec(string? email ) : base(E=> E.Email == email)
        {
            Include.Add(I=>I.Enrollments);   
            Include.Add(I=>I.Submissions);
            Includesting.Add("Enrollments.Course");
        }
    }
}
