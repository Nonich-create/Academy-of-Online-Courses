using Students.DAL.Models;

using System.Threading.Tasks;

namespace Students.BLL.Services
{
    public interface IApplicationCourseService : IBaseService<ApplicationCourse>
    {
        Task Save();
        Task Enroll(ApplicationCourse model);
        Task Cancel(ApplicationCourse model);
        Task DeleteAsyncAll(int id);
    }
}
