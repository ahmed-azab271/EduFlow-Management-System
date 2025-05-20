using Core.Entites;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.IRepository
{
    public interface IGenaricRepo<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync (T entity);
        void Update(T entity);
        void Delete(T entity);

        Task<IReadOnlyList<T>> GetAllIncludedAsync(ISpecification<T> Spec , bool AsNoTraking = false);
        Task<T> GetWhereAndIncludedAsync(ISpecification<T> Spec, bool AsNoTraking = false);

    }
}
