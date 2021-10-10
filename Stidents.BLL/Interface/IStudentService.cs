using Students.DAL.Models;
using System.Threading.Tasks;
using Students.BLL.Interface.Base;
using System.Collections.Generic;

namespace Students.BLL.Interface
{
    public interface IStudentService : IBaseService<Student>
    {
        Task PutRequest(int StudentId, int СourseId);
        Task<Student> GetAsync(int? id);
        Task<IEnumerable<Student>> SearchAllAsync(string query);
    }
}
