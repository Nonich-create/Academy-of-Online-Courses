using Students.BLL.Interface.Base;
using Students.DAL.Enum;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.Interface
{
    public interface ILessonTimesService : IBaseService<LessonTimes>
    {
        Task<int> GetCount(int groupId);
        Task<IEnumerable<LessonTimes>> IndexView(int groupId, int currentPage, int pageSize = 10);
    }
}
