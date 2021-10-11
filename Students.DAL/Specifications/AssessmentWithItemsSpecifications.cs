using Students.DAL.Models;
using Students.DAL.Specifications.Base;


namespace Students.DAL.Specifications
{
    public class AssessmentWithItemsSpecifications : BaseSpecification<Assessment>
    {
        public AssessmentWithItemsSpecifications()
         : base(null)
        {
            AddInclude(a => a.Lesson);
            AddInclude(a => a.Student);
            ApplyOrderBy(a => a.Lesson.NumberLesson);
        }
        public AssessmentWithItemsSpecifications(int currentPage, int pageSize)
: base(null)
        {
            AddInclude(a => a.Lesson);
            AddInclude(a => a.Student);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(a => a.Lesson.NumberLesson);
        }
    }
}