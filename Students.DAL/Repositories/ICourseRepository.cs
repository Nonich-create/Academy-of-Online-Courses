﻿using Students.DAL.Models;
using Students.DAL.Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.DAL.Repositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<IEnumerable<Course>> GetCourseListAsync();
    }
}