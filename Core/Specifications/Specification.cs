using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class Specification<T> : ISpecification<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Where { get ; set ; }
        public List<Expression<Func<T, object>>> Include { get; set; } = new List <Expression<Func<T,object>>>();
        public List<string> Includesting { get; } = new();
        public Expression<Func<T, object>> OrderBy { get; set; }

        public Specification()
        {
        }
        public Specification(Expression<Func<T,bool>> expression)
        {
            Where = expression;
        }
        public void AddOrderBy(Expression<Func<T, object>> expression) => OrderBy = expression;
    }
}
