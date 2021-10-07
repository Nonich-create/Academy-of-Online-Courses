using Students.BLL.Repository.Base;
using Students.DAL.Models;
using Students.DAL.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.Repository
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        private readonly Context _db;

        public CourseRepository(Context db) : base(db)
        {
        }

        public async Task<IEnumerable<Course>> GetCourseListAsync()
        {
             return await GetAllAsync();
        }


    }
}
