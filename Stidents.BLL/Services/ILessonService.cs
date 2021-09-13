using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Threading.Tasks;

namespace Students.BLL.Services
{
    public interface ILessonService : IBaseService<Lesson>
    {
        Task Save();
    }
}
