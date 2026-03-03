using ECommerce.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.IRepositories.Base
{
    public interface IRepository<T> where T : Auditable
    {
        Task<List<T>> GetAllAsync(List<string> includes);
        IQueryable<T> All();
        IEnumerable<T> AllIncludes();
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetById(Guid id, List<string> includes);
        Task<T> AddAsync(T entity);

        Task<List<T>> AddRangeAsync(List<T> entities);

        Task<List<T>> DeleteRangeAsync(List<T> entities);
        Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecification<T> spec);

        Task<T> GetByIdWithSpecAsync(ISpecification<T> spec);

        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<bool> SoftDelete(Guid Id);
        Task<bool> SoftDeleteRangeAsync(List<Guid> entities);

    }
}
