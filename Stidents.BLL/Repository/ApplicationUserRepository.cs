using Microsoft.AspNetCore.Identity;
using Students.BLL.Repository.Base;
using Students.DAL.Models;
using Students.DAL.Repositories;
using System.Threading.Tasks;

namespace Students.BLL.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly Context _db;

        public ApplicationUserRepository(Context db) : base(db)
        {
            _db = db;
        }


        public async Task<ApplicationUser> GetByIdAsync(string id) => await _db.Set<ApplicationUser>().FindAsync(id);
    }
}