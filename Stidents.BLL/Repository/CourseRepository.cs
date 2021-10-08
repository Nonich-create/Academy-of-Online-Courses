using Students.BLL.Repository.Base;
using Students.DAL.Models;
using Students.DAL.Repositories;

namespace Students.BLL.Repository
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        private readonly Context _db;

        public CourseRepository(Context db) : base(db)
        {
            _db = db;
        }

    }
}
