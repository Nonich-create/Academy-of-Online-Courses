using Students.BLL.Interface.Base;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.Interface
{
    public interface IAssessmentService : IBaseService<Assessment>
    {
        Task<IEnumerable<Assessment>> SearchAllAsync(string query);
        Task<IEnumerable<Assessment>> GetAllAsync(int studentId, int courseId);

    }
}
