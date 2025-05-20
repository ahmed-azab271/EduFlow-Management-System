using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class TeacherSpec : Specification<Teacher>
    {
        public TeacherSpec(string TeacherEmail):base(T=>T.Email == TeacherEmail)
        {
            
        }

    }
}
