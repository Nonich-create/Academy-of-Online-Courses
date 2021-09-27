using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.Services
{
    public interface ICourseService : IBaseService<Course>
    {
        Task<IEnumerable<Course>> GetСarouselAsync(int skip = 0);
    }
}
