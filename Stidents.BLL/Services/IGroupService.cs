using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Threading.Tasks;

namespace Students.BLL.Services
{
    public interface IGroupService : IBaseService<Group>
    {
        Task<Group> GetAsync(int? id);
        Task StartGroup(int id);
    }
}
