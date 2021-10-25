using Students.BLL.Interface.Base;
using Students.DAL.Models;
using System.Threading.Tasks;

namespace Students.BLL.Interface
{
    public interface ITeacherService : IBaseService<Teacher>
    {
        Task CreateAsync(Teacher teacher, ApplicationUser user, string password);
    }
}
