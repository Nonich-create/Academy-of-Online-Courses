﻿using Students.DAL.Enum;
using Students.DAL.Models;
using Students.DAL.Specifications.Base;

namespace Students.DAL.Specifications
{
    public class StudentWithItemsSpecifications : BaseSpecification<Student>
    {
        public StudentWithItemsSpecifications()
         : base(null)
        {
            AddInclude(s => s.User);
            AddInclude(s => s.Group);
            AddInclude(s => s.Group.Course);
        
        }
        public StudentWithItemsSpecifications(int currentPage, int pageSize)
      : base(null)
        {
            AddInclude(s => s.User);
            AddInclude(s => s.Group);
            AddInclude(s => s.Group.Course);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(s => s.Surname);
        }
        public StudentWithItemsSpecifications(int currentPage, int pageSize, string stringSearch, EnumSearchParameters searchParametr)
        : base(null)
        {
            AddInclude(s => s.User);
            AddInclude(s => s.Group);
            AddInclude(s => s.Group.Course);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(s => s.Surname);
            ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public StudentWithItemsSpecifications(string stringSearch, EnumSearchParameters searchParametr)
    : base(null)
        {
            AddInclude(s => s.User);
            AddInclude(s => s.Group);
            AddInclude(s => s.Group.Course);
            ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public StudentWithItemsSpecifications(string stringSearch)
  : base(null)
        {
            AddInclude(s => s.User);
            AddInclude(s => s.Group);
            AddInclude(s => s.Group.Course);
            ApplyWhere(stringSearch);
        }
    }
}