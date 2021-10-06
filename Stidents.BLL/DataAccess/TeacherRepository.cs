using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess
{
    public class TeacherRepository : IRepository<Teacher>
    {
        private readonly Context _db;

        public TeacherRepository(Context db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Teacher>> GetAllAsync() =>
            await _db.Teachers.AsQueryable().Include(t => t.User).ToListAsync();
        

        public async Task<Teacher> GetAsync(int id) => await ExistsAsync(id) ? await _db.Teachers.FindAsync(id) : null;

        public async Task CreateAsync(Teacher teacher) => await _db.Teachers.AddAsync(teacher);
 
        public async Task<Teacher> Update(Teacher teacher)
        {
            var teacherEntity = await _db.Teachers.AsQueryable().Include(t => t.User).AsNoTracking().FirstOrDefaultAsync(t => t.Id == teacher .Id);
            if (teacherEntity != null)
            {
                _db.Entry(teacher).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return teacherEntity;
            }
            return teacherEntity;
        }

        public async Task DeleteAsync(int id) => _db.Teachers.Remove(await GetAsync(id));

        public async Task<bool> ExistsAsync(int id) => await _db.Teachers.FindAsync(id) != null;

        public async Task<Teacher> SearchAsync(string query) => await _db.Teachers.AsQueryable().Include(t => t.User).Where(query).FirstAsync();
        
        public async Task CreateRangeAsync(IEnumerable<Teacher> teachers) => await _db.Teachers.AddRangeAsync(teachers);
    }
}
