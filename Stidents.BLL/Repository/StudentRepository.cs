using Students.BLL.Repository.Base;
using Students.DAL.Models;
using Students.DAL.Repositories;

namespace Students.BLL.Repository
{   
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        private readonly Context _db;
        public StudentRepository(Context db) : base(db)
        {
            _db = db;
        }
    }
}