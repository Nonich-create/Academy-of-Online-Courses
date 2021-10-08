using System.Collections.Generic;
using System.Threading.Tasks;
using Students.DAL.Enum;

namespace Students.BLL.Services
{
    public interface IBaseService<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(int id);
        Task CreateAsync(T item);
        Task<T> Update(T item);
        Task DeleteAsync(int id);
        Task<T> SearchAsync(string query);
        Task<IEnumerable<T>> GetPaginatedResult(int currentPage, int pageSize = 10);
        Task<int> GetCount(string searchString, EnumSearchParameters searchParametr);
        Task<IEnumerable<T>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr);
        Task<IEnumerable<T>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize);
        Task<IEnumerable<T>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize);
    }
}
