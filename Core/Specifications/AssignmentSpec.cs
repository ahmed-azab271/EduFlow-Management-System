using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class AssignmentSpec : Specification<Assignment>
    {
        public AssignmentSpec(string? title , DateTime? dateTime ,string? courseName ):base(E=>
        (string.IsNullOrEmpty(title) || E.Title == title)
        &&
        (string.IsNullOrEmpty(courseName) || E.Course.Name == courseName)
        &&
        (!dateTime.HasValue || E.DueDate == dateTime))
        {
            Include.Add(I => I.Course);
            Include.Add(I => I.Submissions);
        }
        public AssignmentSpec(int id ): base (I=>I.Id == id)
        {
            Include.Add(I => I.Course);
            Include.Add(I => I.Submissions);
        }
        public AssignmentSpec(List<string> coursesCodes) : base(I => coursesCodes.Contains(I.Course.Code))
        {
            Include.Add(I => I.Course);
            Include.Add(I => I.Submissions);
        }
        public AssignmentSpec(string title) : base(I => I.Title == title)
        {
            Include.Add(I => I.Course);
            Include.Add(I => I.Submissions);
        }
    }
}
