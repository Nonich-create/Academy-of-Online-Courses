using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Students.BLL.Services
{
    public interface IStudentService : IBaseService<Student>
    {
        Task Save();
        Task PutRequest(int StudentId, int СourseId);
        Task<Student> GetAsync(int? id);
        Task<List<Student>> DisplayingData(string sortRecords, string searchString, int idRecord);
    }
}
