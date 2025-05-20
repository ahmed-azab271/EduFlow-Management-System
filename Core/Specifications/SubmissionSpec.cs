using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class SubmissionSpec : Specification<Submission>
    {
        public SubmissionSpec(int id):base(I=>I.Id == id)
        {
            Include.Add(I=>I.Assignment);
            Include.Add(I=>I.Student);
            Includesting.Add("Assignment.Course");
        }
        public SubmissionSpec(string? email ) : base(I => I.Student.Email == email)
        {
            Include.Add(I => I.Assignment);
            Include.Add(I => I.Student);
            Includesting.Add("Assignment.Course");
        }
        public SubmissionSpec() : base()
        {
            Include.Add(I => I.Assignment);
            Include.Add(I => I.Student);
            Includesting.Add("Assignment.Course");
        }
    }
}
