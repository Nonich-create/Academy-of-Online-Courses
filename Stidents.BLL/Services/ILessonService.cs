using Students.BLL.DataAccess;
using Students.DAL.Enum;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.Services
{
    public interface ILessonService : IBaseService<Lesson>
    {
        Task <bool> CheckRecord(int CourseId, int NumberLesson);
        Task<IEnumerable<Lesson>> DisplayingIndexByIdCourse(int id ,EnumPageActions action, string searchString, EnumSearchParameters searchParametr, int take, int skip = 0);
    }
}
