using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Students.BLL.DataAccess;
using Students.DAL.Enum;
using Microsoft.EntityFrameworkCore.DynamicLinq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic;

namespace Students.BLL.DataAccess
{
    public class StudentRepository : IRepository<Student>
    {
        private readonly Context _db;
        private readonly int skipById = 20;
        private readonly int takeByCount = 10;
        public StudentRepository(Context db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<Student>> GetAllAsync() => await _db.Students.AsQueryable().Include(s => s.Group).ThenInclude(g => g.Course).ToListAsync();

        public async Task<Student> GetAsync(int id) => await ExistsAsync(id) ? await _db.Students.FindAsync(id) : null;

        public async Task<Student> GetAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }
            return await ExistsAsync((int)id) ? await _db.Students.FindAsync(id) : null;
        }

        public async Task CreateAsync(Student student)
        {
            await _db.Students.AddAsync(student);
            await _db.SaveChangesAsync();
        }

        public async Task CreateRangeAsync(IEnumerable<Student> students)
        {
            await _db.Students.AddRangeAsync(students);
            await _db.SaveChangesAsync();
        }

        public async Task<Student> Update(Student student)
        {
            var studentEntity = await _db.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == student.Id);

            if (studentEntity != null)
            {
                _db.Entry(student).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return studentEntity;
            }

            return studentEntity;
        }

        public async Task DeleteAsync(int id)
        {
            Student student = await GetAsync(id);
            if (student != null)
            {
                _db.Students.Remove(student);
                await _db.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(int id) => await _db.Students.FindAsync(id) != null;

        public async Task<Student> SearchAsync(string predicate)
        {
            return await _db.Students.Include(s => s.Group).ThenInclude(g => g.Course).Where(predicate).FirstAsync();
        }

        public async Task<IEnumerable<Student>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, EnumPageActions action, int take, int skip = 0)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return null;
            if (action == EnumPageActions.Add)
                return await _db.Students.AsQueryable().Include(s => s.Group).ThenInclude(g => g.Course)
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take+ takeByCount).ToListAsync();
            return await _db.Students.AsQueryable().Include(s => s.Group).ThenInclude(g => g.Course)
             .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take).ToListAsync();
        }
        public async Task<IEnumerable<Student>> GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            if (action == EnumPageActions.Next)
                return await _db.Students.AsQueryable().Include(s => s.Group).ThenInclude(g => g.Course).Skip(skip).Take(take).ToListAsync();
             
            if (action == EnumPageActions.Back)
            {
                skip = (skip < skipById) ? 20 : skip;
                return await _db.Students.AsQueryable().Include(s => s.Group).ThenInclude(g => g.Course).Skip(skip - skipById).Take(take).ToListAsync();
            }

            return await _db.Students.AsQueryable().Include(s => s.Group).ThenInclude(g => g.Course).Skip(skip).Take(take).ToListAsync();

        }
    }
}
