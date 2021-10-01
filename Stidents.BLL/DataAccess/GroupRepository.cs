using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess
{
    public class GroupRepository : IRepository<Group>
    {
        private readonly int skipById = 20;
        private readonly int takeByCount = 10;
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
        public async Task CreateAsync(Group group)
        {
            await _db.Groups.AddAsync(group);
            await _db.SaveChangesAsync();
        }

        public async Task CreateRangeAsync(IEnumerable<Group> groups)
        {
            await _db.Groups.AddRangeAsync(groups);
            await _db.SaveChangesAsync();
        }

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

        public async Task DeleteAsync(int id)
        {
            Group group = await GetAsync(id);
            if (group != null)
            {
                _db.Groups.Remove(group);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id) => await _db.Groups.FindAsync(id) != null;
        
        public async Task<bool> ExistsAsync(int? id) => await _db.Groups.FindAsync(id) != null;


        public async Task<Group> SearchAsync(string predicate)
        {
            return await _db.Groups.Include(g => g.Course).Include(g => g.Teacher).Include(g => g.Manager).Where(predicate).FirstAsync();
        }

        public async Task<IEnumerable<Group>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, EnumPageActions action, int take, int skip = 0)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return null;
            if (action == EnumPageActions.Add)
                return await _db.Groups.AsQueryable().Include(g => g.Course).Include(g => g.Teacher).Include(g => g.Manager)
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take + takeByCount).ToListAsync();
            return await _db.Groups.AsQueryable().Include(g => g.Course).Include(g => g.Teacher).Include(g => g.Manager)
             .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<IEnumerable<Group>> GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            if (action == EnumPageActions.Next)
                return await _db.Groups.AsQueryable().Include(g => g.Course).Include(g => g.Teacher).Include(g => g.Manager).Skip(skip).Take(take).ToListAsync();

            if (action == EnumPageActions.Back)
            {
                skip = (skip < skipById) ? 20 : skip;
                return await _db.Groups.AsQueryable().Include(g => g.Course).Include(g => g.Teacher).Include(g => g.Manager).Skip(skip - skipById).Take(take).ToListAsync();
            }
            return await _db.Groups.AsQueryable().Include(g => g.Course).Include(g => g.Teacher).Include(g => g.Manager).Skip(skip).Take(take).ToListAsync();
        }
    }
}
