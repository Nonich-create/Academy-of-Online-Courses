using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.DataAccess
{
    public class LessonRepository : IRepository<Lesson>
    {
        private readonly Context _db;

        public LessonRepository(Context db)
        {
            this._db = db;
        }

        public async Task<List<Lesson>> GetAllAsync() => await _db.Lessons.ToListAsync();
        

        public async Task<Lesson> GetAsync(int id) => await ExistsAsync(id) ? await _db.Lessons.FindAsync(id) : null;
        

        public async Task CreateAsync(Lesson lesson) =>  await _db.Lessons.AddAsync(lesson);

        public async Task<Lesson> Update(Lesson lesson)
        {
            var lessonEntity = await _db.Lessons.AsNoTracking().FirstOrDefaultAsync(l => l.LessonId == lesson.LessonId);

            if (lessonEntity != null)
            {
                _db.Entry(lesson).State = EntityState.Modified;

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
            }
        }
        public async Task<bool> ExistsAsync(int id) => await _db.Lessons.FindAsync(id) != null;
        
    }
}
