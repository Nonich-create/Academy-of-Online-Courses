using Students.DAL.Models;
using System.Threading.Tasks;
using Students.BLL.Interface.Base;
using System.Collections.Generic;

namespace Students.BLL.Interface
{
    public interface ICourseApplicationService : IBaseService<CourseApplication>
    {
        Task Enroll(CourseApplication model);
        Task Cancel(CourseApplication model);
        Task<IEnumerable<CourseApplication>> SearchAllAsync(string query);
        Task DeleteAsyncAll(int id);
    }
}
