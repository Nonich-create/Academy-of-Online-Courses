using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;
using System;

namespace Students.BLL.DataAccess
{
    public class AssessmentRepository : IRepository<Assessment>
    {
        private readonly int skipById = 20;
        private readonly int takeByCount = 10;
        private readonly Context _db;

        public AssessmentRepository(Context db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<Assessment>> GetAllAsync() => await _db.Assessments.AsQueryable().Include(a => a.Student).Include(a => a.Lesson).ToListAsync();

        public async Task<Assessment> GetAsync(int id) => await ExistsAsync(id) ? await _db.Assessments.FindAsync(id) : null;

        public async Task CreateAsync(Assessment assessment)
        {
            await _db.Assessments.AddAsync(assessment);
            await _db.SaveChangesAsync();
        }

        public async Task CreateRangeAsync(IEnumerable<Assessment> assessments)
        {
            
            await _db.Assessments.AddRangeAsync(assessments);
            await _db.SaveChangesAsync();
        }

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

        public async Task DeleteAsync(int id)
        {
            Assessment assessment = await GetAsync(id);
            if (assessment != null)
            {
                 _db.Assessments.Remove(assessment);
                await _db.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(int id) => await _db.Assessments.FindAsync(id) != null;

        public async Task<Assessment> SearchAsync(string predicate)
        {
            return await _db.Assessments.Include(a => a.Student).Include(a => a.Lesson).Where(predicate).FirstAsync();
        }

        public async Task<IEnumerable<Assessment>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, EnumPageActions action, int take, int skip = 0)
        {
            if (action == EnumPageActions.Add)
                return await _db.Assessments.AsQueryable().Include(a => a.Student).Include(a => a.Lesson)
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take + takeByCount).ToListAsync();

            return await _db.Assessments.AsQueryable().Include(a => a.Student).Include(a => a.Lesson)
             .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<IEnumerable<Assessment>> GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            if (action == EnumPageActions.Next)
                return await _db.Assessments.AsQueryable().Include(a => a.Student).Include(a => a.Lesson).Skip(skip).Take(take).ToListAsync();

            if (action == EnumPageActions.Back)
            {
                skip = (skip < skipById) ? 20 : skip;
                return await _db.Assessments.AsQueryable().Include(a => a.Student).Include(a => a.Lesson).Skip(skip - skipById).Take(take).ToListAsync();
            }

            return await _db.Assessments.AsQueryable().Include(a => a.Student).Include(a => a.Lesson).Skip(skip).Take(take).ToListAsync();

        }

    }
}
