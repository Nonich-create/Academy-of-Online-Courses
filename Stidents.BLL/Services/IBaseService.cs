using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        Task<bool> ExistsAsync(int id);
        Task<T> SearchAsync(string predicate);
        Task<IEnumerable<T>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, EnumPageActions action, int take, int skip = 0);
        Task<IEnumerable<T>> GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0);
        Task<IEnumerable<T>> DisplayingIndex(EnumPageActions action, string searchString, EnumSearchParameters searchParametr, int take, int skip = 0);
    }
}
