using Students.DAL.Models;
using System.Threading.Tasks;
using Students.BLL.Interface.Base;
using System.Collections.Generic;
using System.IO;

namespace Students.BLL.Interface
{
    public interface IStudentService : IBaseService<Student>
    {
        Task PutRequest(int StudentId, int СourseId);
        Task<Student> GetAsync(int? id);
        Task<IEnumerable<Student>> SearchAllAsync(string query);
        Task<Stream> GetContent(int studentId);
        Task<IEnumerable<Student>> GetAllAsync(int groupId);
        Task CreateAsync(Student student, ApplicationUser user, string password);
    }
 
}
