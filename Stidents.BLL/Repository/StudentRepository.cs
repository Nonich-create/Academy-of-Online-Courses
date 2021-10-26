using Students.BLL.Repository.Base;
using Students.DAL.Models;
using Students.DAL.Repositories;

namespace Students.BLL.Repository
{   
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(Context db) : base(db)
        {
        }
    }
}