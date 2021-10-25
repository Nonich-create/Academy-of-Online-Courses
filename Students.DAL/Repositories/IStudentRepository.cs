using Students.DAL.Models;
using Students.DAL.Repositories.Base;

namespace Students.DAL.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
    }
}
