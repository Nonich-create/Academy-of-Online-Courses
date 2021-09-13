using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Threading.Tasks;

namespace Students.BLL.Services
{
    public interface ICourseService : IBaseService<Course>
    {
        Task Save();
    }
}
