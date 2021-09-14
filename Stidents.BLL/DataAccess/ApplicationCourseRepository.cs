using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Students.BLL.DataAccess   
{
    public class ApplicationCourseRepository : IRepository<CourseApplication>
    {
        private readonly Context _db;
        
        public ApplicationCourseRepository(Context db)
        {
            this._db = db;
        }
        
        public async Task<List<CourseApplication>> GetAllAsync() => await _db.ApplicationCourses.ToListAsync();      
        
        public async Task<CourseApplication> GetAsync(int id) => await ExistsAsync(id) ? await _db.ApplicationCourses.FindAsync(id) : null;
        
        public async Task CreateAsync(CourseApplication courseApplication) => await _db.ApplicationCourses.AddAsync(courseApplication);
             
        public async Task<CourseApplication> Update(CourseApplication courseApplication)
        {
            var applicationCoursesEntity = await _db.ApplicationCourses
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
                _db.ApplicationCourses.Remove(courseApplication);
            }
        }
        public async Task DeleteAsyncAll(int id)
        {
            Student students = await _db.Students.FindAsync(id);
            if (students != null)
            {
                var applicationCourses = await GetAllAsync();
                applicationCourses = applicationCourses.Where(a => a.StudentId == id).ToList();
                _db.ApplicationCourses.RemoveRange(applicationCourses);
            }
        }

        public async Task<bool> ExistsAsync(int id) => await _db.ApplicationCourses.FindAsync(id) != null;
    }
}
