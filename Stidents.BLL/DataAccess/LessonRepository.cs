using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess
{
    public class LessonRepository : IRepository<Lesson>
    {
        private readonly Context _db;

        public LessonRepository(Context db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<Lesson>> GetAllAsync() => await _db.Lessons.Include(l => l.Course).ToListAsync();
        
        public async Task<Lesson> GetAsync(int id) => await ExistsAsync(id) ? await _db.Lessons
            .Include(l => l.Course).FirstOrDefaultAsync(c => c.Id == id) : null;

        public async Task CreateAsync(Lesson lesson) => await _db.Lessons.AddAsync(lesson);

        public async Task CreateRangeAsync(IEnumerable<Lesson> lessons) => await _db.Lessons.AddRangeAsync(lessons);

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

        public async Task DeleteAsync(int id) => _db.Lessons.Remove(await GetAsync(id));

        public async Task<bool> ExistsAsync(int id) => await _db.Lessons.FindAsync(id) != null;

        public async Task<Lesson> SearchAsync(string query) => await _db.Lessons.Include(l => l.Course).Where(query).FirstAsync();
        
    }
}
