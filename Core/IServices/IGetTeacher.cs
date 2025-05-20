using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.IServices
{
    public interface  IGetTeacher
    {
        public Task<Teacher> GetTeacherAsync(string TeacherEmail);
    }
}
