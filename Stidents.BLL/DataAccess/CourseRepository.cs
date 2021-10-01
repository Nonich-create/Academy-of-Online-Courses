using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess
{
    public class CourseRepository : IRepository<Course>
    {
        private readonly int skipById = 20;
        private readonly int takeByCount = 10;
        private readonly Context _db;

        public CourseRepository(Context db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<Course>> GetAllAsync() => await _db.Courses.ToListAsync();
        
        public async Task<Course> GetAsync(int id) => await ExistsAsync(id) ? await _db.Courses.FindAsync(id) : null;

        public async Task CreateAsync(Course course)
        {
            await _db.Courses.AddAsync(course);
            await _db.SaveChangesAsync();
        }

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

        public async Task DeleteAsync(int id)
        {
            Course course = await GetAsync(id);
            if (course != null)
            {
                _db.Courses.Remove(course);
                await _db.SaveChangesAsync();
            }
        }

        public async Task CreateRangeAsync(IEnumerable<Course> courses)
        {
            await _db.Courses.AddRangeAsync(courses);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id) => await _db.Courses.FindAsync(id) != null;

        public async Task<Course> SearchAsync(string predicate)
        {
            return await _db.Courses.Where(predicate).FirstAsync();
        }

        public async Task<IEnumerable<Course>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, EnumPageActions action, int take, int skip = 0)
        {
            if (string.IsNullOrEmpty(searchString))
                return null;
            if (action == EnumPageActions.Add)
                return await _db.Courses.AsQueryable()
                .Where($"Name.Contains(@0)", searchString).Skip(skip).Take(take + takeByCount).ToListAsync();
            return await _db.Courses.AsQueryable()
             .Where($"Name.Contains(@0)", searchString).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            if (action == EnumPageActions.Next)
                return await _db.Courses.AsQueryable().Skip(skip).Take(take).ToListAsync();

            if (action == EnumPageActions.Back)
            {
                skip = (skip < skipById) ? 20 : skip;
                return await _db.Courses.AsQueryable().Skip(skip - skipById).Take(take).ToListAsync();
            }
            return await _db.Courses.AsQueryable().Skip(skip).Take(take).ToListAsync();
        }

    }
}
