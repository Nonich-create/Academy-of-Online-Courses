using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess
{
    public class TeacherRepository : IRepository<Teacher>
    {
        private readonly int skipById = 20;
        private readonly int takeByCount = 10;
        private readonly Context _db;

        public TeacherRepository(Context db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Teacher>> GetAllAsync() =>
            await _db.Teachers.AsQueryable().Include(t =>t.Groups).ThenInclude(g =>g.Course).ToListAsync();
        

        public async Task<Teacher> GetAsync(int id) => await ExistsAsync(id) ? await _db.Teachers.FindAsync(id) : null;

        public async Task CreateAsync(Teacher teacher)
        {
            await _db.Teachers.AddAsync(teacher);
            await _db.SaveChangesAsync();
        }
        
        public async Task<Teacher> Update(Teacher teacher)
        {
            var teacherEntity = await _db.Teachers.AsQueryable().AsNoTracking().FirstOrDefaultAsync(t => t.Id == teacher .Id);

            if (teacherEntity != null)
            {
                _db.Entry(teacher).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return teacherEntity;
            }

            return teacherEntity;
        }

        public async Task DeleteAsync(int id)
        {
            Teacher teacher = await GetAsync(id);
            if (teacher != null)
            {
                _db.Teachers.Remove(teacher);
                await _db.SaveChangesAsync();
            }   
        }
        public async Task<bool> ExistsAsync(int id) => await _db.Teachers.FindAsync(id) != null;

        public async Task<IEnumerable<Teacher>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, EnumPageActions action, int take, int skip = 0)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.none)
                return null;
            if (action == EnumPageActions.add)
                return await _db.Teachers.AsQueryable()
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take + takeByCount).ToListAsync();
            return await _db.Teachers.AsQueryable()
             .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take).ToListAsync();

        }

        public async Task CreateRangeAsync(IEnumerable<Teacher> teachers)
        {
            await _db.Teachers.AddRangeAsync(teachers);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Teacher>>  GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            if (action == EnumPageActions.next)
                return await _db.Teachers.AsQueryable().Skip(skip).Take(take).ToListAsync();

            if (action == EnumPageActions.back)
            {
                skip = (skip < skipById) ? 20 : skip;
                return await _db.Teachers.AsQueryable().Skip(skip - skipById).Take(take).ToListAsync();
            }
            return  await _db.Teachers.AsQueryable().Skip(skip).Take(take).ToListAsync();
        }
               

    }
}
