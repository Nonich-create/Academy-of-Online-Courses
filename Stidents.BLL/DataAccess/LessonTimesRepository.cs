using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess
{
    public class LessonTimesRepository : IRepository<LessonTimes>
    {
        private readonly Context _db;

        public LessonTimesRepository(Context db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<LessonTimes>> GetAllAsync() => await _db.LessonTimes
            .AsQueryable().Include(l => l.Group).Include(l =>l.Lesson).ToListAsync();

        public async Task<LessonTimes> GetAsync(int id) => await ExistsAsync(id) ? await _db.LessonTimes.FindAsync(id) : null;

        public async Task CreateAsync(LessonTimes lessonTimes)
        {
            await _db.LessonTimes.AddAsync(lessonTimes);
            await _db.SaveChangesAsync();
        }

        public async Task CreateRangeAsync(IEnumerable<LessonTimes> lessonesTimes)
        {
            await _db.LessonTimes.AddRangeAsync(lessonesTimes);
            await _db.SaveChangesAsync();
        }

        public async Task<LessonTimes> Update(LessonTimes lessonTimes)
        {
            var lessonEntity = await _db.LessonTimes.AsNoTracking().FirstOrDefaultAsync(l => l.Id == lessonTimes.Id);
            if (lessonEntity != null)
            {
                _db.Entry(lessonTimes).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return lessonEntity;
            }
            return lessonEntity;
        }

        public async Task DeleteAsync(int id)
        {
            LessonTimes lessonTimes = await GetAsync(id);
            if (lessonTimes != null)
            {
                _db.LessonTimes.Remove(lessonTimes);
                await _db.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(int id) => await _db.LessonTimes.FindAsync(id) != null;

        public async Task<LessonTimes> SearchAsync(string query)
        {
            return await _db.LessonTimes.Include(l => l.Group).Include(l => l.Lesson).Where(query).FirstAsync();
        }
    }
}
