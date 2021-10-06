using Students.DAL.Models;
using System.Threading.Tasks;

namespace Students.BLL.Services
{
    public interface ICourseApplicationService : IBaseService<CourseApplication>
    {
        Task Enroll(CourseApplication model);
        Task Cancel(CourseApplication model);
        Task DeleteAsyncAll(int id);
    }
}
