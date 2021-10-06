using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess
{
    public class AssessmentRepository : IRepository<Assessment>
    {
        private readonly Context _db;

        public AssessmentRepository(Context db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<Assessment>> GetAllAsync() => 
            await _db.Assessments.AsQueryable().Include(a => a.Student).Include(a => a.Lesson).ToListAsync();

        public async Task<Assessment> GetAsync(int id) => await ExistsAsync(id) ? await _db.Assessments.FindAsync(id) : null;

        public async Task CreateAsync(Assessment assessment) => await _db.Assessments.AddAsync(assessment);
        
        public async Task CreateRangeAsync(IEnumerable<Assessment> assessments) => await _db.Assessments.AddRangeAsync(assessments);


        public async Task<Assessment> Update(Assessment assessment)
        {
            var assessmentEntity = await _db.Assessments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == assessment.Id);
            if (assessmentEntity != null)
            {
                _db.Entry(assessment).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return assessmentEntity;
            }
            return assessmentEntity;
        }

        public async Task DeleteAsync(int id) => _db.Assessments.Remove(await GetAsync(id));

        public async Task<bool> ExistsAsync(int id) => await _db.Assessments.FindAsync(id) != null;

        public async Task<Assessment> SearchAsync(string query) => await _db.Assessments.Include(a => a.Student).Include(a => a.Lesson).Where(query).FirstAsync();

    }
}
