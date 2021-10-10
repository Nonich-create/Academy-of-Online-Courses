using Students.DAL.Enum;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Students.BLL.Interface.Base;

namespace Students.BLL.Interface
{
    public interface ILessonService : IBaseService<Lesson>
    {
        Task <bool> CheckRecord(int CourseId, int NumberLesson);
        Task<IEnumerable<Lesson>> IndexView(int courseId, string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize);
    }
}
