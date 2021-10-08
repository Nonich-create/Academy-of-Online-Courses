using Students.BLL.Repository.Base;
using Students.DAL.Models;
using Students.DAL.Repositories;
using Students.DAL.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.Repository
{
    public class AssessmentRepository : Repository<Assessment>, IAssessmentRepository
    {
        public AssessmentRepository(Context db) : base(db)
        {
        }
        public async Task<IEnumerable<Assessment>> GetAssessmentsListAsync()
        {
            var spec = new AssessmentWithItemsSpecifications();
            return await GetAsync(spec);
        }
    }
}