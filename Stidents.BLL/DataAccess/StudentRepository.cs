using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Students.DAL.Enum;
using Microsoft.EntityFrameworkCore.DynamicLinq;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess
{
    public class StudentRepository : IRepository<Student>
    {
        private readonly Context _db;
        public StudentRepository(Context db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<Student>> GetAllAsync() => 
            await _db.Students.AsQueryable().Include(s => s.Group).ThenInclude(g => g.Course).ToListAsync();

        public async Task<Student> GetAsync(int id) => await ExistsAsync(id) ? await _db.Students.FindAsync(id) : null;

        public async Task<Student> GetAsync(int? id) => await ExistsAsync((int)id) ? await _db.Students.FindAsync(id) : null;
     
        public async Task CreateAsync(Student student) => await _db.Students.AddAsync(student);

        public async Task CreateRangeAsync(IEnumerable<Student> students) => await _db.Students.AddRangeAsync(students);

        public async Task<Student> Update(Student student)
        {
            var studentEntity = await _db.Students.Include(t => t.User).AsNoTracking().FirstOrDefaultAsync(t => t.Id == student.Id);
            if (studentEntity != null)
            {
                _db.Entry(student).State = EntityState.Modified;
                return studentEntity;
            }
            return studentEntity;
        }

        public async Task DeleteAsync(int id) => _db.Students.Remove(await GetAsync(id));
        
        public async Task<bool> ExistsAsync(int id) => await _db.Students.FindAsync(id) != null;

        public async Task<Student> SearchAsync(string query)
        {
            return await _db.Students.AsQueryable().Include(s => s.Group).ThenInclude(g => g.Course).Where(query).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Student>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            return await _db.Students.AsQueryable().Include(s => s.Group).ThenInclude(g => g.Course)
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString)
                .ToListAsync();
        }
    }
}