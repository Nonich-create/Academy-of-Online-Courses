using Students.BLL.Repository.Base;
using Students.DAL.Models;
using Students.DAL.Repositories;
using Students.DAL.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.Repository
{
    public class ManagerRepository : Repository<Manager>, IManagerRepository
    {
        public ManagerRepository(Context db) : base(db)
        {
        }
        public async Task<IEnumerable<Manager>> GetManagerListAsync()
        {
            var spec = new ManagerWithItemsSpecifications();
            return await GetAsync(spec);
        }

    }
}