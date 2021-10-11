using Students.BLL.Repository.Base;
using Students.DAL.Models;
using Students.DAL.Repositories;
using Students.DAL.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.Repository
{   
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        private readonly Context _db;
        public StudentRepository(Context db) : base(db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Student>> GetStudentListAsync()
        {
            var spec = new StudentWithItemsSpecifications();
            return await GetAsync(spec);
        }
        public async Task<IEnumerable<Student>> GetStudentListAsync(int currentPage, int pageSize)
        {
            var spec = new StudentWithItemsSpecifications(currentPage, pageSize);
            return await GetAsync(spec);
        }

    }
}