using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess
{
    public class LessonRepository : IRepository<Lesson>
    {
        private readonly int skipById = 20;
        private readonly int takeByCount = 10;
        private readonly Context _db;

        public LessonRepository(Context db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<Lesson>> GetAllAsync() => await _db.Lessons.Include(l => l.Course).ToListAsync();
        

        public async Task<Lesson> GetAsync(int id) => await ExistsAsync(id) ? await _db.Lessons.FindAsync(id) : null;


        public async Task CreateAsync(Lesson lesson)
        {
            await _db.Lessons.AddAsync(lesson);
            await _db.SaveChangesAsync();
        }

        public async Task CreateRangeAsync(IEnumerable<Lesson> lessons)
        {
            await _db.Lessons.AddRangeAsync(lessons);
            await _db.SaveChangesAsync();
        }

        public async Task<Lesson> Update(Lesson lesson)
        {
            var lessonEntity = await _db.Lessons.AsNoTracking().FirstOrDefaultAsync(l => l.Id == lesson.Id);
            if (lessonEntity != null)
            {
                _db.Entry(lesson).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return lessonEntity;
            }
            return lessonEntity;
        }

        public async Task DeleteAsync(int id)
        {
            Lesson lesson = await GetAsync(id);
            if (lesson != null)
            {
                _db.Lessons.Remove(lesson);
                await _db.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(int id) => await _db.Lessons.FindAsync(id) != null;

        public async Task<IEnumerable<Lesson>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, EnumPageActions action, int take, int skip = 0)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.none)
                return null;
            if (action == EnumPageActions.add)
                return await _db.Lessons.AsQueryable().Include(l => l.Course)
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take + takeByCount).ToListAsync();

            return await _db.Lessons.AsQueryable().Include(l => l.Course)
             .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<IEnumerable<Lesson>>  GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            if (action == EnumPageActions.next)
                return await _db.Lessons.AsQueryable().Include(l => l.Course).Skip(skip).Take(take).ToListAsync();

            if (action == EnumPageActions.back)
            {
                skip = (skip < skipById) ? 20 : skip;
                return await _db.Lessons.AsQueryable().Include(l => l.Course).Skip(skip - skipById).Take(take).ToListAsync();
            }
            return await _db.Lessons.AsQueryable().Include(l => l.Course).Skip(skip).Take(take).ToListAsync();
        }
    }
}
