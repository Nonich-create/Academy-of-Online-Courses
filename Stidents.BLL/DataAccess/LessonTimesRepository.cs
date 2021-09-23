using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess
{
    public class LessonTimesRepository : IRepository<LessonTimes>
    {
        private readonly int skipById = 20;
        private readonly int takeByCount = 10;
        private readonly Context _db;

        public LessonTimesRepository(Context db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<LessonTimes>> GetAllAsync() => await _db.LessonTimes
            .AsQueryable().Include(l => l.Group).Include(l =>l.Lesson).ToListAsync();

        public async Task<LessonTimes> GetAsync(int id) => await ExistsAsync(id) ? await _db.LessonTimes.FindAsync(id) : null;

        public async Task CreateAsync(LessonTimes lesson)
        {
            await _db.LessonTimes.AddAsync(lesson);
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

        public async Task<IEnumerable<LessonTimes>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, EnumPageActions action, int take, int skip = 0)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.none)
                return null;
            if (action == EnumPageActions.add)
                return await _db.LessonTimes.AsQueryable().Include(l => l.Group).Include(l => l.Lesson)
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take + takeByCount).ToListAsync();

            return await _db.LessonTimes.AsQueryable().Include(l => l.Group).Include(l => l.Lesson)
             .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<IEnumerable<LessonTimes>>  GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            if (action == EnumPageActions.next)
                return await _db.LessonTimes.AsQueryable().Include(l => l.Group).Include(l => l.Lesson).Skip(skip).Take(take).ToListAsync();

            if (action == EnumPageActions.back)
            {
                skip = (skip < skipById) ? 20 : skip;
                return await _db.LessonTimes.AsQueryable().Include(l => l.Group).Include(l => l.Lesson).Skip(skip - skipById).Take(take).ToListAsync();
            }
            return await _db.LessonTimes.AsQueryable().Include(l => l.Group).Include(l => l.Lesson).Skip(skip).Take(take).ToListAsync();
        }
               
    }
}
