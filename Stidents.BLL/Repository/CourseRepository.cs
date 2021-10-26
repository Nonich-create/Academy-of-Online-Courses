using Students.BLL.Repository.Base;
using Students.DAL.Models;
using Students.DAL.Repositories;

namespace Students.BLL.Repository
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(Context db) : base(db)
        {
        }

    }
}
