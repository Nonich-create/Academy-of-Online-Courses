using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess   
{
    public class CourseApplicationRepository : IRepository<CourseApplication>
    {
                private readonly int skipById = 20;
        private readonly int takeByCount = 10;
        private readonly Context _db;
        
        public CourseApplicationRepository(Context db)
        {
            this._db = db;
        }
        
        public async Task<IEnumerable<CourseApplication>> GetAllAsync() => await _db.CourseApplication.AsQueryable()
            .Include(c => c.Course).Include(c => c.Student).ToListAsync();      
        
        public async Task<CourseApplication> GetAsync(int id) => await ExistsAsync(id) ? await _db.CourseApplication.FindAsync(id) : null;
        
        public async Task CreateAsync(CourseApplication courseApplication) => await _db.CourseApplication.AddAsync(courseApplication);

        public async Task CreateRangeAsync(IEnumerable<CourseApplication> courseApplications)
        {
            await _db.CourseApplication.AddRangeAsync(courseApplications);
            await _db.SaveChangesAsync();
        }

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
        
        public async Task DeleteAsync(int id)
        {
            CourseApplication courseApplication = await GetAsync(id);
            if (courseApplication != null)
            {
                _db.CourseApplication.Remove(courseApplication);
                await _db.SaveChangesAsync();
            }
        }
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

        public async Task<CourseApplication> SearchAsync(string predicate)
        {
            return await _db.CourseApplication.Include(c => c.Course).Include(c => c.Student).Where(predicate).FirstAsync();
        }

        public async Task<IEnumerable<CourseApplication>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, EnumPageActions action, int take, int skip = 0)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return null;
            if (action == EnumPageActions.Add)
                return await _db.CourseApplication.AsQueryable().Include(c => c.Course).Include(c => c.Student)
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take + takeByCount).ToListAsync();
            return await _db.CourseApplication.AsQueryable().Include(c => c.Course).Include(c => c.Student)
             .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<IEnumerable<CourseApplication>> GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            if (action == EnumPageActions.Next)
                return await _db.CourseApplication.AsQueryable().Include(c => c.Course).Include(c => c.Student).Skip(skip).Take(take).ToListAsync();

            if (action == EnumPageActions.Back)
            {
                skip = (skip < skipById) ? 20 : skip;
                return await _db.CourseApplication.AsQueryable().Include(c => c.Course).Include(c => c.Student).Skip(skip - skipById).Take(take).ToListAsync();
            }
            return await _db.CourseApplication.AsQueryable().Include(c => c.Course).Include(c => c.Student).Skip(skip).Take(take).ToListAsync();
        }
    }
}
