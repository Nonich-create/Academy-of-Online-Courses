using Microsoft.AspNetCore.Identity;
using Students.BLL.Repository.Base;
using Students.DAL.Models;
using Students.DAL.Repositories;
using System.Threading.Tasks;

namespace Students.BLL.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationUserRepository(Context db) : base(db)
        {
        }
    }
}