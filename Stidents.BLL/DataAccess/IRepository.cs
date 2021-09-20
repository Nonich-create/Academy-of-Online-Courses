using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Students.BLL.DataAccess
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(int id);
        Task CreateAsync(T item);
        Task<T> Update(T item);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
