using Students.BLL.Repository.Base;
using Students.DAL.Models;
using Students.DAL.Repositories;
using Students.DAL.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.Repository
{
    public class LessonTimesRepository : Repository<LessonTimes>, ILessonTimesRepository
    {
        public LessonTimesRepository(Context db) : base(db)
        {
        }
        public async Task<IEnumerable<LessonTimes>> GetLessonTimesListAsync()
        {
            var spec = new LessonTimesWithItemsSpecifications();
            return await GetAsync(spec);
        }

    }
}