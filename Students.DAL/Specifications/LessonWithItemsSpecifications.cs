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
    }
}