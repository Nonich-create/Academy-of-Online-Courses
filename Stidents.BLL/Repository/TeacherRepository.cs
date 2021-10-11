using Students.BLL.Repository.Base;
using Students.DAL.Models;
using Students.DAL.Repositories;
using Students.DAL.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.Repository
{
    public class TeacherRepository : Repository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(Context db) : base(db)
        {
        }

        public async Task<IEnumerable<Teacher>> GetTeacherListAsync( )
        {
            var spec = new TeacherWithItemsSpecifications();
            return await GetAsync(spec);
        }

        public async Task<IEnumerable<Teacher>> GetTeacherListAsync(int currentPage, int pageSize)
        {
            var spec = new TeacherWithItemsSpecifications(currentPage,pageSize);
            return await GetAsync(spec);
        }
    }
}