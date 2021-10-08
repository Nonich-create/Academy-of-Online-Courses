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
    }
}