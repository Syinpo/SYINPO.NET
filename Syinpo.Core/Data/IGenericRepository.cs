using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syinpo.Core.Data {
    public interface IGenericRepository<TEntity> where TEntity : class {
        Task<TEntity> GetById(int id);
        Task Create(TEntity entity);
        Task CreateRange(List<TEntity> entityList);
        Task Update(TEntity entity);
        Task UpdateRange(List<TEntity> entityList);
        Task Delete(TEntity entity);
        Task DeleteRange(List<TEntity> entityList);
        IQueryable<TEntity> Table { get; }
        IQueryable<TEntity> WriteTable { get; }
        IQueryable<TEntity> AutoTable {
            get;
        }
    }
}
