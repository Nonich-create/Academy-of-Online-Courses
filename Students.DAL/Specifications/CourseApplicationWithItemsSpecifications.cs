﻿using Students.DAL.Models;
using Students.DAL.Specifications.Base;


namespace Students.DAL.Specifications
{
    public class CourseApplicationWithItemsSpecifications : BaseSpecification<CourseApplication>
    {
        public CourseApplicationWithItemsSpecifications()
         : base(null)
        {
            AddInclude(c => c.Student);
            AddInclude(c => c.Course);
        }
            public CourseApplicationWithItemsSpecifications(int currentPage, int pageSize)
: base(null)
        {
            AddInclude(c => c.Student);
            AddInclude(c => c.Course);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(c => c.Course.Name);
        }
    }
}