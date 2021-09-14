using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Students.BLL.DataAccess;

namespace Students.BLL.DataAccess
{
    public class StudentRepository : IRepository<Student>
    {
        private readonly Context _db;

        public StudentRepository(Context db)
        {
            this._db = db;
        }

        public async Task<List<Student>> GetAllAsync() => await _db.Students.ToListAsync();
        
        public async Task<Student> GetAsync(int id) => await ExistsAsync(id) ? await _db.Students.FindAsync(id) : null;
        
        public async Task<Student> GetAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }
            return await ExistsAsync((int)id) ? await _db.Students.FindAsync(id) : null;
        }
        public async Task CreateAsync(Student student)=> await _db.Students.AddAsync(student);
        
        public async Task<Student> Update(Student student)
        {
            var studentEntity = await _db.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == student .Id);

            if (studentEntity != null)
            {
                _db.Entry(student).State = EntityState.Modified;

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
            }
        }
        public async Task<bool> ExistsAsync(int id) => await _db.Students.FindAsync(id) != null;
    }
}
