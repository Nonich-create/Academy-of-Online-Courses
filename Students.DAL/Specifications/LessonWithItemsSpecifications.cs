using Students.DAL.Models;
using Students.DAL.Specifications.Base;


namespace Students.DAL.Specifications
{
    public class LessonWithItemsSpecifications : BaseSpecification<Lesson>
    {
        public LessonWithItemsSpecifications()
         : base(null)
        {
            AddInclude(l => l.Course);
        }
        public LessonWithItemsSpecifications(int currentPage, int pageSize)
: base(null)
        {
            AddInclude(l => l.Course);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(l => l.NumberLesson );
        }
    }
}