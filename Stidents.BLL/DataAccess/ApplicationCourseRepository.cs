using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Students.BLL.DataAccess   
{
    public class ApplicationCourseRepository : IRepository<ApplicationCourse>
    {
        private readonly Context _db;
        
        public ApplicationCourseRepository(Context db)
        {
            this._db = db;
        }
        
        public async Task<List<ApplicationCourse>> GetAllAsync() => await _db.ApplicationCourses.ToListAsync();      
        
        public async Task<ApplicationCourse> GetAsync(int id) => await ExistsAsync(id) ? await _db.ApplicationCourses.FindAsync(id) : null;
        
        public async Task CreateAsync(ApplicationCourse applicationCourse) => await _db.ApplicationCourses.AddAsync(applicationCourse);
             
        public async Task<ApplicationCourse> Update(ApplicationCourse applicationCourse)
        {
            var applicationCoursesEntity = await _db.ApplicationCourses
                .FirstOrDefaultAsync(a => a.ApplicationCourseId == applicationCourse.ApplicationCourseId);
            if (applicationCoursesEntity != null)
            {
                //_db.Entry(applicationCourse).State = EntityState.Modified;   
                _db.ApplicationCourses.Update(applicationCoursesEntity);
            }
            return applicationCoursesEntity;
        }
        
        public async Task DeleteAsync(int id)
        {
            ApplicationCourse applicationCourse = await GetAsync(id);
            if (applicationCourse != null)
            {
                _db.ApplicationCourses.Remove(applicationCourse);
            }
        }
        public async Task DeleteAllForStudentAsync(int studentId)
        {
            Student student = await _db.Students.FindAsync(studentId);
            if (student != null)
            {
                var applicationCourses = (await GetAllAsync()).Where(a => a.StudentId == studentId).ToList();
                _db.ApplicationCourses.RemoveRange(applicationCourses);
            }
        }

        public async Task<bool> ExistsAsync(int id) => await _db.ApplicationCourses.AnyAsync(a => a.ApplicationCourseId == id);
    }
}
