using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess
{
    public class ManagerRepository : IRepository<Manager>
    {
        private readonly int skipById = 20;
        private readonly int takeByCount = 10;
        private readonly Context _db;

        public ManagerRepository(Context db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<Manager>> GetAllAsync() => await _db.Manager.ToListAsync();
        
        public async Task<Manager> GetAsync(int id) => await ExistsAsync(id) ? await _db.Manager.FindAsync(id) : null;

        public async Task CreateAsync(Manager manager)
        {
            await _db.Manager.AddAsync(manager);
            await _db.SaveChangesAsync();
        }

        public async Task CreateRangeAsync(IEnumerable<Manager> managers)
        {
            await _db.Manager.AddRangeAsync(managers);
            await _db.SaveChangesAsync();
        }

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

        public async Task DeleteAsync(int id)
        {
            Manager manager = await GetAsync(id);
            if (manager != null)
            {
                _db.Manager.Remove(manager);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id) => await _db.Manager.FindAsync(id) != null;

        public async Task<IEnumerable<Manager>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, EnumPageActions action, int take, int skip = 0)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.none)
                return null;
            if (action == EnumPageActions.add)
                return await _db.Manager.AsQueryable()
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take + takeByCount).ToListAsync();

            return await _db.Manager.AsQueryable()
             .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<IEnumerable<Manager>> GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            if (action == EnumPageActions.next)
                return await _db.Manager.AsQueryable().Skip(skip).Take(take).ToListAsync();

            if (action == EnumPageActions.back)
            { 
            skip = (skip < skipById) ? 20 : skip;
            return await _db.Manager.AsQueryable().Skip(skip).Take(take).ToListAsync();
            }
            return await _db.Manager.AsQueryable().Skip(skip).Take(take).ToListAsync();
        }
                

    }
}
