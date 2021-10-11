using Students.BLL.Repository.Base;
using Students.DAL.Models;
using Students.DAL.Repositories;
using Students.DAL.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Students.BLL.Repository
{
    public class CourseApplicationRepository : Repository<CourseApplication>, ICourseApplicationRepository
    {
        private readonly Context _db;

        public CourseApplicationRepository(Context db) : base(db)
        {
            _db = db;
        }

        public async Task DeleteAsyncAllByStudentId(int StudentId)
        {
                _db.CourseApplication.RemoveRange((await GetAllAsync()).Where(a => a.StudentId == StudentId));
        }

        public async Task<IEnumerable<CourseApplication>> GetCourseApplicationListAsync()
        {
            var spec = new CourseApplicationWithItemsSpecifications();
            return await GetAsync(spec);
        }

        public async Task<IEnumerable<CourseApplication>> GetCourseApplicationListAsync(int currentPage, int pageSize)
        {
            var spec = new CourseApplicationWithItemsSpecifications(currentPage,pageSize);
            return await GetAsync(spec);
        }
    }
}