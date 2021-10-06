using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess   
{
    public class CourseApplicationRepository : IRepository<CourseApplication>
    {
        private readonly Context _db;
        
        public CourseApplicationRepository(Context db)
        {
            this._db = db;
        }
        
        public async Task<IEnumerable<CourseApplication>> GetAllAsync() => await _db.CourseApplication.AsQueryable()
            .Include(c => c.Course).Include(c => c.Student).ToListAsync();      
        
        public async Task<CourseApplication> GetAsync(int id) => await ExistsAsync(id) ? await _db.CourseApplication.FindAsync(id) : null;
        
        public async Task CreateAsync(CourseApplication courseApplication) => await _db.CourseApplication.AddAsync(courseApplication);

        public async Task CreateRangeAsync(IEnumerable<CourseApplication> courseApplications) => await _db.CourseApplication.AddRangeAsync(courseApplications);

        public async Task<CourseApplication> Update(CourseApplication courseApplication)
        {
            var applicationCoursesEntity = await _db.CourseApplication
                .AsNoTracking().FirstOrDefaultAsync(a => a.Id == courseApplication.Id);
            if (applicationCoursesEntity != null)
            {
                _db.Entry(courseApplication).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return applicationCoursesEntity;
            }
            return applicationCoursesEntity;
        }
        
        public async Task DeleteAsync(int id) => _db.CourseApplication.Remove(await GetAsync(id));

        public async Task DeleteAsyncAll(int id)
        {
            Student students = await _db.Students.FindAsync(id);
            if (students != null)
            {
                _db.CourseApplication.RemoveRange((await GetAllAsync()).Where(a => a.StudentId == id));
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id) => await _db.CourseApplication.FindAsync(id) != null;

        public async Task<CourseApplication> SearchAsync(string query) =>
             await _db.CourseApplication.Include(c => c.Course).Include(c => c.Student).Where(query).FirstAsync();

    }
}
