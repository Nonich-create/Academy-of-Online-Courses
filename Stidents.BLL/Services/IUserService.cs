using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Threading.Tasks;

namespace Students.BLL.Services
{
    public interface IUserService : IBaseService<ApplicationUser>
    {
        Task Save();
        Task<bool> ExistsAsync(string id);
        Task<ApplicationUser> GetAsync(string id);
    }
}
