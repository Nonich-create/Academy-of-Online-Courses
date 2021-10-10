using Students.DAL.Models;
using System.Threading.Tasks;
using Students.BLL.Interface.Base;

namespace Students.BLL.Interface
{
    public interface IUserService : IBaseService<ApplicationUser>
    {
        Task<ApplicationUser> GetAsync(string id);
        Task DeleteAsync(string id);
    }
}
