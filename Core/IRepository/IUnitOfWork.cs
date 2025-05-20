using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.IRepository
{
    public interface IUnitOfWork : IAsyncDisposable
    { 
        IGenaricRepo<T> Entity<T>() where T : BaseEntity;
        Task<int> SaveAsync();
    }
}
