using Students.BLL.Repository.Base;
using Students.DAL.Models;
using Students.DAL.Repositories;
using Students.DAL.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.Repository
{
    public class LessonRepository : Repository<Lesson>, ILessonRepository
    {
        public LessonRepository(Context db) : base(db)
        {
        }
        public async Task<IEnumerable<Lesson>> GetLessonListAsync()
        {
            var spec = new LessonWithItemsSpecifications();
            return await GetAsync(spec);
        }
        public async Task<IEnumerable<Lesson>> GetLessonListAsync(int currentPage, int pageSize)
        {
            var spec = new LessonWithItemsSpecifications(currentPage,pageSize);
            return await GetAsync(spec);
        }

    }
}