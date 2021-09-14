using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.DataAccess
{
    public class LessonPlanRepository : IRepository<LessonPlan>
    {
        private readonly Context _db;

        public LessonPlanRepository(Context db)
        {
            this._db = db;
        }

        public async Task<List<LessonPlan>> GetAllAsync() => await _db.LessonPlans.ToListAsync();


        public async Task<LessonPlan> GetAsync(int id) => await ExistsAsync(id) ? await _db.LessonPlans.FindAsync(id) : null;


        public async Task CreateAsync(LessonPlan lesson) => await _db.LessonPlans.AddAsync(lesson);

        public async Task<LessonPlan> Update(LessonPlan lessonPlan)
        {
            var lessonEntity = await _db.LessonPlans.AsNoTracking().FirstOrDefaultAsync(l => l.Id == lessonPlan.Id);

            if (lessonEntity != null)
            {
                _db.Entry(lessonPlan).State = EntityState.Modified;

                return lessonEntity;
            }

            return lessonEntity;
        }

        public async Task DeleteAsync(int id)
        {
            LessonPlan lessonPlan = await GetAsync(id);
            if (lessonPlan != null)
            {
                _db.LessonPlans.Remove(lessonPlan);
            }
        }
        public async Task<bool> ExistsAsync(int id) => await _db.LessonPlans.FindAsync(id) != null;
    }
}
