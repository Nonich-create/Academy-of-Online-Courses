using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Students.BLL.DataAccess
{
    public class GroupRepository : IRepository<Group>
    {
        private readonly Context _db;

        public GroupRepository(Context db)
        {
            this._db = db;
        }

        public async Task<List<Group>> GetAllAsync() => await _db.Groups.ToListAsync();
        
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
        
        public async Task<Group> Update(Group group)
        {
            var groupEntity = await _db.Groups.AsNoTracking().FirstOrDefaultAsync(g => g.GroupId == group.GroupId);

            if (groupEntity != null)
            {
                _db.Entry(group).State = EntityState.Modified;

                return groupEntity;
            }

            return groupEntity;
        }

        public async Task DeleteAsync(int id)
        {
            Group group = await GetAsync(id);
            if (group != null)
            {
                _db.Groups.Remove(group);
            }
        }

        public async Task<bool> ExistsAsync(int id) => await _db.Groups.FindAsync(id) != null;
        
        public async Task<bool> ExistsAsync(int? id) => await _db.Groups.FindAsync(id) != null;
        

    }
}
