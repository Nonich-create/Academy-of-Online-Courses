using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Students.BLL.DataAccess   
{
    public class CourseApplicationRepository : IRepository<CourseApplication>
    {
        private readonly Context _db;
        
        public CourseApplicationRepository(Context db)
        {
            this._db = db;
        }
        
        public async Task<IEnumerable<CourseApplication>> GetAllAsync() => await _db.CourseApplication.ToListAsync();      
        
        public async Task<CourseApplication> GetAsync(int id) => await ExistsAsync(id) ? await _db.CourseApplication.FindAsync(id) : null;
        
        public async Task CreateAsync(CourseApplication courseApplication) => await _db.CourseApplication.AddAsync(courseApplication);
             
        public async Task<CourseApplication> Update(CourseApplication courseApplication)
        {
            var applicationCoursesEntity = await _db.CourseApplication
                .AsNoTracking().FirstOrDefaultAsync(a => a.Id == courseApplication.Id);
            if (applicationCoursesEntity != null)
            {
                _db.Entry(courseApplication).State = EntityState.Modified;
        
                return applicationCoursesEntity;
            }
            return applicationCoursesEntity;
        }
        
        public async Task DeleteAsync(int id)
        {
            CourseApplication courseApplication = await GetAsync(id);
            if (courseApplication != null)
            {
                _db.CourseApplication.Remove(courseApplication);
            }
        }
        public async Task DeleteAsyncAll(int id)
        {
            Student students = await _db.Students.FindAsync(id);
            if (students != null)
            {
                _db.CourseApplication.RemoveRange((await GetAllAsync()).Where(a => a.StudentId == id));
            }
        }

        public async Task<bool> ExistsAsync(int id) => await _db.CourseApplication.FindAsync(id) != null;
    }
}
