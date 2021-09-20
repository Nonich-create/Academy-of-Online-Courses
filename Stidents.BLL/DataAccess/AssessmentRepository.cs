using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.DataAccess
{
    public class AssessmentRepository : IRepository<Assessment>
    {
        private readonly Context _db;

        public AssessmentRepository(Context db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<Assessment>> GetAllAsync() => await _db.Assessments.ToListAsync();
        

        public async Task<Assessment> GetAsync(int id) => await ExistsAsync(id) ? await _db.Assessments.FindAsync(id) : null;
        

        public async Task CreateAsync(Assessment assessment) => await _db.Assessments.AddAsync(assessment);
        

        public async Task<Assessment> Update(Assessment assessment)
        {
            var assessmentEntity = await _db.Assessments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == assessment.Id);

            if (assessmentEntity != null)
            {
                _db.Entry(assessment).State = EntityState.Modified;

                return assessmentEntity;
            }

            return assessmentEntity;
        }

        public async Task DeleteAsync(int id)
        {
            Assessment assessment = await GetAsync(id);

            if (assessment != null)
            {
                 _db.Assessments.Remove(assessment);
            }
        }
        public async Task<bool> ExistsAsync(int id) => await _db.Assessments.FindAsync(id) != null;
        
    }
}
