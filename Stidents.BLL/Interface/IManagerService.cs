using Students.BLL.Interface.Base;
using Students.DAL.Models;
using System.Threading.Tasks;

namespace Students.BLL.Interface
{
    public interface IManagerService : IBaseService<Manager>
    {
        Task CreateAsync(Manager manager, ApplicationUser user, string password);
    }
}
