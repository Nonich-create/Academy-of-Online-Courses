using Students.BLL.Repository.Base;
using Students.DAL.Models;
using Students.DAL.Repositories;
using Students.DAL.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.Repository
{
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        public GroupRepository(Context db) : base(db)
        {
        }

        public async Task<IEnumerable<Group>> GetGroupsListAsync()
        {
            var spec = new GroupWithItemsSpecifications();
            return await GetAsync(spec);
        }
 
    }
}