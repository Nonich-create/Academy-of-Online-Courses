using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Students.BLL.DataAccess
{
    public class ApplicationUserRepository: IRepository<ApplicationUser>
    {
        private readonly Context _db;

        public ApplicationUserRepository(Context db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync() => await _db.ApplicationUsers.ToListAsync();

        public async Task<ApplicationUser> GetAsync(int id) => await _db.ApplicationUsers.FindAsync(id.ToString());

        public async Task<ApplicationUser> GetAsync(string id) => await _db.ApplicationUsers.FindAsync(id);

        public async Task CreateAsync(ApplicationUser applicationUser) => await _db.ApplicationUsers.AddAsync(applicationUser);

        public async Task CreateRangeAsync(IEnumerable<ApplicationUser> applicationUsers) => await _db.ApplicationUsers.AddRangeAsync(applicationUsers);

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
            }
        }
        public async Task DeleteAsync(string id)
        {
            ApplicationUser applicationUser = await GetAsync(id);
            if (applicationUser != null)
            {
                _db.ApplicationUsers.Remove(applicationUser);
            }
        }
        public async Task<bool> ExistsAsync(string id) => await GetAsync(id) != null;
        
        public async Task<bool> ExistsAsync(int id) => await _db.ApplicationUsers.FindAsync(id) != null;

        public async Task<ApplicationUser> SearchAsync(string query)
        {
            return await _db.ApplicationUsers.Where(query).FirstAsync();
        }
    }
}
