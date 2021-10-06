using Students.DAL.Models;
using System.Threading.Tasks;

namespace Students.BLL.Services
{
    public interface IStudentService : IBaseService<Student>
    {
        Task PutRequest(int StudentId, int СourseId);
        Task<Student> GetAsync(int? id);
    }
}
