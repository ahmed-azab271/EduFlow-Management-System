using Core.Entites;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public static class SpecificationEvaluoator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery (IQueryable<T> InitialQuery , ISpecification<T> Spec , bool AsNoTraking)
        {
            var Query = InitialQuery;
            if (Spec.Where is not null) 
                Query = Query.Where (Spec.Where);

            if(Spec.OrderBy is not null)
                Query = Query.OrderBy (Spec.OrderBy);

            Query = Spec.Include.Aggregate(Query , (Input, Output) => Input.Include(Output));
            Query = Spec.Includesting.Aggregate(Query , (Current , Include) => Current.Include(Include));

            if (AsNoTraking)
                Query.AsNoTracking();

            return Query;
        }
    }
}
