using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public interface ISpecification<T> where T : BaseEntity
    {
        public Expression<Func<T,bool>> Where { get; set; }
        public List<Expression<Func<T,object>>> Include { get; set; }
        public List<string> Includesting { get; }
        public Expression<Func<T, object>> OrderBy { get; set; }
    }
}
