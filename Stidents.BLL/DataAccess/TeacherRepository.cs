using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.DataAccess
{
    public class TeacherRepository : IRepository<Teacher>
    {
        private readonly Context _db;

        public TeacherRepository(Context db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Teacher>> GetAllAsync() => await _db.Teachers.ToListAsync();
        

        public async Task<Teacher> GetAsync(int id) => await ExistsAsync(id) ? await _db.Teachers.FindAsync(id) : null;
        
        public async Task CreateAsync(Teacher teacher) => await _db.Teachers.AddAsync(teacher);
        
        public async Task<Teacher> Update(Teacher teacher)
        {
            var teacherEntity = await _db.Teachers.AsNoTracking().FirstOrDefaultAsync(t => t.Id == teacher .Id);

            if (teacherEntity != null)
            {
                _db.Entry(teacher).State = EntityState.Modified;

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
            }   
        }
        public async Task<bool> ExistsAsync(int id) => await _db.Teachers.FindAsync(id) != null;
        
    }
}
