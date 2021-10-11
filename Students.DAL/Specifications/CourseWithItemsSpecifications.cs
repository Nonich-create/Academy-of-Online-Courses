using Students.DAL.Enum;
using Students.DAL.Models;
using Students.DAL.Specifications.Base;


namespace Students.DAL.Specifications
{
    public class CourseWithItemsSpecifications : BaseSpecification<Course>
    {
        public CourseWithItemsSpecifications()
         : base(null)
        {
        }
            public CourseWithItemsSpecifications(int currentPage, int pageSize)
: base(null)
        {
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(c => c.Name);
        }

        public CourseWithItemsSpecifications(int currentPage, int pageSize, string stringSearch, EnumSearchParameters searchParametr)
: base(null)
        {;
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(c => c.Name);
            ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public CourseWithItemsSpecifications(string stringSearch, EnumSearchParameters searchParametr)
    : base(null)
        {
            ApplyOrderBy(c => c.Name);
            ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public CourseWithItemsSpecifications(string stringSearch)
    : base(null)
        {
            ApplyOrderBy(c => c.Name); ;
            ApplyWhere(stringSearch);
        }
    }
}