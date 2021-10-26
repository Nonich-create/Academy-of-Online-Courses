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
            ApplyOrderBy(c => c.ApplicationStatus);
        }
        public CourseApplicationWithItemsSpecifications(int currentPage, int pageSize, string stringSearch, EnumSearchParameters searchParametr)
: base(null)
        {
            AddInclude(c => c.Student);
            AddInclude(c => c.Course);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(c => c.Id);
            if (!string.IsNullOrEmpty(stringSearch) && searchParametr != EnumSearchParameters.None)
                ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public CourseApplicationWithItemsSpecifications(string stringSearch, EnumSearchParameters searchParametr)
    : base(null)
        {
            AddInclude(c => c.Student);
            AddInclude(c => c.Course);
            ApplyOrderBy(c => c.Course.Name);
            if (!string.IsNullOrEmpty(stringSearch) && searchParametr != EnumSearchParameters.None)
            {
                ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
            }
        }

        public CourseApplicationWithItemsSpecifications(string stringSearch)
    : base(null)
        {
            AddInclude(c => c.Student);
            AddInclude(c => c.Course);
            ApplyOrderBy(c => c.Course.Name);
            ApplyWhere(stringSearch);
        }

        public CourseApplicationWithItemsSpecifications(uint studentId, uint courseId)
    : base(c => c.StudentId == studentId && c.CourseId == courseId)
        {
        }
    }
}