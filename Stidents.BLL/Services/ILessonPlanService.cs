using Students.DAL.Models;
using System.Threading.Tasks;

namespace Students.BLL.Services
{
    public interface ILessonPlanService : IBaseService<LessonPlan>
    {
        Task Save();
    }
}
