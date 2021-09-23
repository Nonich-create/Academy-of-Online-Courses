using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Students.BLL.Services
{
    public interface IAssessmentService : IBaseService<Assessment>
    {
 
        Task <IEnumerable<Assessment>> GetAssessmentsByStudentId(int studentId);
    }
}
