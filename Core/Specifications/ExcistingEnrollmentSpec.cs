using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class ExcistingEnrollmentSpec : Specification<Enrollment>
    {
        public ExcistingEnrollmentSpec(int? studenId, int? courseId) : base(E =>
        (studenId == null || E.StudentId == studenId)
        &&
        (courseId == null || E.CourseId == courseId)
        )
        {
            Include.Add(I => I.Student);
            Include.Add(I => I.Course);
        }
        public ExcistingEnrollmentSpec(int? enrollmentId) : base(I => I.Id == enrollmentId)
        {
            Include.Add(I => I.Student);
            Include.Add(I => I.Course);
        }
    }
}
