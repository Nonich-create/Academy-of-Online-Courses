using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess
{
    public class GroupRepository : IRepository<Group>
    {
        private readonly Context _db;

        public GroupRepository(Context db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<Group>> GetAllAsync() => await _db.Groups
            .AsQueryable().Include(g =>g.Course).Include(g => g.Teacher).Include(g => g.Manager).ToListAsync();
        
        public async Task<Group> GetAsync(int id) => await ExistsAsync(id) ? await _db.Groups.FindAsync(id) : null;
        
        public async Task<Group> GetAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }
            return await ExistsAsync(id) ? await _db.Groups.FindAsync(id) : null;
        }
        public async Task CreateAsync(Group group) => await _db.Groups.AddAsync(group);

        public async Task CreateRangeAsync(IEnumerable<Group> groups) => await _db.Groups.AddRangeAsync(groups);

        public async Task<Group> Update(Group group)
        {
            var groupEntity = await _db.Groups.AsNoTracking().FirstOrDefaultAsync(g => g.Id == group.Id);
            if (groupEntity != null)
            {
                _db.Entry(group).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return groupEntity;
            }
            return groupEntity;
        }

        public async Task DeleteAsync(int id) => _db.Groups.Remove(await GetAsync(id));

        public async Task<bool> ExistsAsync(int id) => await _db.Groups.FindAsync(id) != null;
        
        public async Task<bool> ExistsAsync(int? id) => await _db.Groups.FindAsync(id) != null;

        public async Task<Group> SearchAsync(string query) => await _db.Groups.Include(g => g.Course).Include(g => g.Teacher).Include(g => g.Manager).Where(query).FirstAsync();
        
    }
}
