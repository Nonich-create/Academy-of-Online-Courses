using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Threading.Tasks;

namespace Students.BLL.Services
{
    public interface IStudentService : IBaseService<Student>
    {
        Task Save();
        Task PutRequest(int StudentId, int СourseId);
        Task<Student> GetAsync(int? id);
    }
}
