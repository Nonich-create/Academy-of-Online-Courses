using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess
{
    public class ApplicationUserRepository: IRepository<ApplicationUser>
    {
        private readonly int skipById = 20;
        private readonly int takeByCount = 10;
        private readonly Context _db;

        public ApplicationUserRepository(Context db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync() => await _db.ApplicationUsers.ToListAsync();

        public async Task<ApplicationUser> GetAsync(int id) => await _db.ApplicationUsers.FindAsync(id.ToString());

        public async Task<ApplicationUser> GetAsync(string id) => await _db.ApplicationUsers.FindAsync(id);

        public async Task CreateAsync(ApplicationUser applicationUser)
        {
            await _db.ApplicationUsers.AddAsync(applicationUser);
            await _db.SaveChangesAsync();
        }

        public async Task CreateRangeAsync(IEnumerable<ApplicationUser> applicationUsers)
        {
            await _db.ApplicationUsers.AddRangeAsync(applicationUsers);
            await _db.SaveChangesAsync();
        }

        public async Task<ApplicationUser> Update(ApplicationUser applicationUser)
        {
            var applicationUserEntity = await _db.ApplicationUsers.FirstOrDefaultAsync(a => a.Id == applicationUser.Id);
            if (applicationUserEntity != null)
            {
                _db.Entry(applicationUser).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return applicationUserEntity;
            }
            return applicationUserEntity;
        }

        public async Task DeleteAsync(int id)
        {
            ApplicationUser applicationUser = await GetAsync(id);
            if (applicationUser != null)
            {
                _db.ApplicationUsers.Remove(applicationUser);
                await _db.SaveChangesAsync();
            }
        }
        public async Task DeleteAsync(string id)
        {
            ApplicationUser applicationUser = await GetAsync(id);
            if (applicationUser != null)
            {
                _db.ApplicationUsers.Remove(applicationUser);
                await _db.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(string id) => await GetAsync(id) != null;
        
        public async Task<bool> ExistsAsync(int id) => await _db.ApplicationUsers.FindAsync(id) != null;

        public async Task<IEnumerable<ApplicationUser>> GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            if (action == EnumPageActions.Next)
                return await _db.ApplicationUsers.Skip(skip).Take(take).ToListAsync();

            if (action == EnumPageActions.Back)
            { 
                skip = (skip < skipById) ? 20 : skip;
 
                return await _db.ApplicationUsers.Skip(skip - skipById).Take(take).ToListAsync();
            }

            return await _db.ApplicationUsers.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<ApplicationUser> SearchAsync(string predicate)
        {
            return await _db.ApplicationUsers.Where(predicate).FirstAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, EnumPageActions action, int take, int skip = 0)
        {
            if (action == EnumPageActions.Add)
                return await _db.ApplicationUsers.AsQueryable()
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take + takeByCount).ToListAsync();

            return await _db.ApplicationUsers.AsQueryable()
             .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString).Skip(skip).Take(take).ToListAsync();

        }
    }
}
