using Core.Entites;
using Core.IRepository;
using Core.IServices;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class GetTeacher : IGetTeacher
    {
        private readonly IUnitOfWork unitOfWork;

        public GetTeacher(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public async Task<Teacher> GetTeacherAsync(string TeacherEmail)
        {
            var Spec = new TeacherSpec(TeacherEmail);
            var Teacher = await unitOfWork.Entity<Teacher>().GetWhereAndIncludedAsync(Spec);
            return Teacher;
        }
    }
}
