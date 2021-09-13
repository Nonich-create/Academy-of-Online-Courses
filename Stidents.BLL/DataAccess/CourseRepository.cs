using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Students.BLL.DataAccess
{
    public class CourseRepository : IRepository<Course>
    {
        private readonly Context _db;

        public CourseRepository(Context db)
        {
            this._db = db;
        }

        public async Task<List<Course>> GetAllAsync() => await _db.Courses.ToListAsync();
        
        public async Task<Course> GetAsync(int id) => await ExistsAsync(id) ? await _db.Courses.FindAsync(id) : null;

        public async Task CreateAsync(Course course) => await _db.Courses.AddAsync(course); 

        public async Task<Course> Update(Course course)
        {
            var courseEntity = await _db.Courses.AsNoTracking().FirstOrDefaultAsync(c=>c.CourseId ==course.CourseId);
            if (courseEntity != null)
            {
               _db.Entry(course).State = EntityState.Modified;
                return courseEntity;
            }

            return courseEntity;
        }

        public async Task DeleteAsync(int id)
        {
            Course course = await GetAsync(id);
            if (course != null)
            {
                _db.Courses.Remove(course);
            }
        }
        public async Task<bool> ExistsAsync(int id) => await _db.Courses.FindAsync(id) != null;
        
    }
}
