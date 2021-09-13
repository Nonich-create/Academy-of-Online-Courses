using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.DataAccess
{
    public class ManagerRepository : IRepository<Manager>
    {
        private readonly Context _db;

        public ManagerRepository(Context db)
        {
            this._db = db;
        }

        public async Task<List<Manager>> GetAllAsync() => await _db.Manager.ToListAsync();
        
        public async Task<Manager> GetAsync(int id) => await ExistsAsync(id) ? await _db.Manager.FindAsync(id) : null;
        
        public async Task CreateAsync(Manager manager) => await _db.Manager.AddAsync(manager);
        
        public async Task<Manager> Update(Manager manager)
        {
            var methodologistEntity = await _db.Manager.AsNoTracking().FirstOrDefaultAsync(a => a.ManagerId == manager.ManagerId);

            if (methodologistEntity != null)
            {
                _db.Entry(manager).State = EntityState.Modified;

                return methodologistEntity;
            }

            return methodologistEntity;
        }

        public async Task DeleteAsync(int id)
        {
            Manager manager = await GetAsync(id);
            if (manager != null)
            {
                _db.Manager.Remove(manager);
            }
        }

        public async Task<bool> ExistsAsync(int id) => await _db.Manager.FindAsync(id) != null;
        
    }
}
