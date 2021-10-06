using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess
{
    public class CourseRepository : IRepository<Course>
    {
        private readonly Context _db;

        public CourseRepository(Context db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<Course>> GetAllAsync() => await _db.Courses.ToListAsync();
        
        public async Task<Course> GetAsync(int id) => await ExistsAsync(id) ? await _db.Courses.FindAsync(id) : null;

        public async Task CreateAsync(Course course) => await _db.Courses.AddAsync(course);
        
        public async Task<Course> Update(Course course)
        {
            var courseEntity = await _db.Courses.AsNoTracking().FirstOrDefaultAsync(c=>c.Id == course.Id);
            if (courseEntity != null)
            {
                _db.Entry(course).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return courseEntity;
            }

            return courseEntity;
        }

        public async Task DeleteAsync(int id) => _db.Courses.Remove(await GetAsync(id));

        public async Task CreateRangeAsync(IEnumerable<Course> courses) => await _db.Courses.AddRangeAsync(courses);

        public async Task<bool> ExistsAsync(int id) => await _db.Courses.FindAsync(id) != null;

        public async Task<Course> SearchAsync(string query) => await _db.Courses.Where(query).FirstAsync();
     
    }
}
