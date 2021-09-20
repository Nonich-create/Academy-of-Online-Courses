using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.DataAccess
{
    public class LessonTimesRepository : IRepository<LessonTimes>
    {
        private readonly Context _db;

        public LessonTimesRepository(Context db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<LessonTimes>> GetAllAsync() => await _db.LessonTimes.ToListAsync();


        public async Task<LessonTimes> GetAsync(int id) => await ExistsAsync(id) ? await _db.LessonTimes.FindAsync(id) : null;


        public async Task CreateAsync(LessonTimes lesson) => await _db.LessonTimes.AddAsync(lesson);

        public async Task<LessonTimes> Update(LessonTimes lessonPlan)
        {
            var lessonEntity = await _db.LessonTimes.AsNoTracking().FirstOrDefaultAsync(l => l.Id == lessonPlan.Id);

            if (lessonEntity != null)
            {
                _db.Entry(lessonPlan).State = EntityState.Modified;

                return lessonEntity;
            }

            return lessonEntity;
        }

        public async Task DeleteAsync(int id)
        {
            LessonTimes lessonPlan = await GetAsync(id);
            if (lessonPlan != null)
            {
                _db.LessonTimes.Remove(lessonPlan);
            }
        }
        public async Task<bool> ExistsAsync(int id) => await _db.LessonTimes.FindAsync(id) != null;
    }
}
