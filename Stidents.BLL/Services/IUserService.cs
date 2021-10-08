using Students.DAL.Models;
using System.Threading.Tasks;

namespace Students.BLL.Services
{
    public interface IUserService : IBaseService<ApplicationUser>
    {
        Task<ApplicationUser> GetAsync(string id);
        Task DeleteAsync(string id);
    }
}
