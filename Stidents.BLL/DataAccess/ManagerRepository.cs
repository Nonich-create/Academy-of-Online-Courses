using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess
{
    public class ManagerRepository : IRepository<Manager>
    {
        private readonly Context _db;

        public ManagerRepository(Context db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<Manager>> GetAllAsync() => 
            await _db.Manager.AsQueryable().Include(m => m.User).ToListAsync();

        public async Task<Manager> GetAsync(int id) => await ExistsAsync(id) ? await _db.Manager.FindAsync(id) : null;

        public async Task CreateAsync(Manager manager) => await _db.Manager.AddAsync(manager);

        public async Task CreateRangeAsync(IEnumerable<Manager> managers) => await _db.Manager.AddRangeAsync(managers);

        public async Task<Manager> Update(Manager manager)
        {
            var methodologistEntity = await _db.Manager.AsNoTracking().FirstOrDefaultAsync(a => a.Id == manager.Id);
            if (methodologistEntity != null)
            {
                _db.Entry(manager).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return methodologistEntity;
            }
            return methodologistEntity;
        }

        public async Task DeleteAsync(int id) => _db.Manager.Remove(await GetAsync(id));
        
        public async Task<bool> ExistsAsync(int id) => await _db.Manager.FindAsync(id) != null;

        public async Task<Manager> SearchAsync(string query) => await _db.Manager.Where(query).FirstAsync();
    }
}
