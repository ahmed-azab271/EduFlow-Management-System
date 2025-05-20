using Core.Entites;
using Core.IRepository;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class GenaricRepo<T> : IGenaricRepo<T> where T : BaseEntity
    {
        private readonly AppDbContext dbcontext;

        public GenaricRepo(AppDbContext _dbcontext)
        {
            dbcontext = _dbcontext;
        }
        public async Task<IReadOnlyList<T>> GetAllAsync() => await dbcontext.Set<T>().ToListAsync();
        public async Task<T> GetByIdAsync(int id) => await dbcontext.Set<T>().FindAsync(id);
        public async Task AddAsync(T entity)=> await dbcontext.Set<T>().AddAsync(entity);
        public  void Update(T entity) =>  dbcontext.Set<T>().Update(entity);
        public void Delete(T entity) => dbcontext.Set<T>().Remove(entity);

        public async Task<IReadOnlyList<T>> GetAllIncludedAsync(ISpecification<T> Spec, bool AsNoTraking)
            => await SpecificationEvaluoator<T>.GetQuery(dbcontext.Set<T>() , Spec , AsNoTraking).ToListAsync();

        public async Task<T> GetWhereAndIncludedAsync(ISpecification<T> Spec, bool AsNoTraking)
            => await SpecificationEvaluoator<T>.GetQuery(dbcontext.Set<T>(), Spec , AsNoTraking).FirstOrDefaultAsync();
    }
}
