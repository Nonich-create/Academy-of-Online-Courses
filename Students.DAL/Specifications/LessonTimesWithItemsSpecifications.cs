using Students.DAL.Models;
using Students.DAL.Specifications.Base;


namespace Students.DAL.Specifications
{
    public class LessonTimesWithItemsSpecifications : BaseSpecification<LessonTimes>
    {
        public LessonTimesWithItemsSpecifications()
         : base(null)
        {
            AddInclude(l => l.Lesson);
            AddInclude(l => l.Group);
        }

        public LessonTimesWithItemsSpecifications(int currentPage, int pageSize)
: base(null)
        {
            AddInclude(l => l.Lesson);
            AddInclude(l => l.Group);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(l => l.Group.NumberGroup);
        }
    }
}