using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Students.DAL.Enum;

namespace Students.BLL.Services
{
    public interface IStudentService : IBaseService<Student>
    {
        Task PutRequest(int StudentId, int СourseId);
        Task<Student> GetAsync(int? id);
    }
}
