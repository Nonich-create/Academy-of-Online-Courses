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
        }
    }
}