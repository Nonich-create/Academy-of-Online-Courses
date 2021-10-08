using Students.DAL.Models;
using Students.DAL.Repositories.Base;
using System.Threading.Tasks;

namespace Students.DAL.Repositories
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser> GetByIdAsync(string id);
    }
}
