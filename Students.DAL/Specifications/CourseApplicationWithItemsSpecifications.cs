using Students.DAL.Enum;
using Students.DAL.Models;
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

        public CourseApplicationWithItemsSpecifications(int studentId)
     : base(c => c.StudentId == studentId)
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
            ApplyOrderBy(c => c.Id);
        }
        public CourseApplicationWithItemsSpecifications(int currentPage, int pageSize, string stringSearch, EnumSearchParameters searchParametr)
: base(null)
        {
            AddInclude(c => c.Student);
            AddInclude(c => c.Course);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(c => c.Id);
            ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public CourseApplicationWithItemsSpecifications(string stringSearch, EnumSearchParameters searchParametr)
    : base(null)
        {
            AddInclude(c => c.Student);
            AddInclude(c => c.Course);
            ApplyOrderBy(c => c.Course.Name);
            ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public CourseApplicationWithItemsSpecifications(string stringSearch)
    : base(null)
        {
            AddInclude(c => c.Student);
            AddInclude(c => c.Course);
            ApplyOrderBy(c => c.Course.Name);
            ApplyWhere(stringSearch);
        }
    }
}