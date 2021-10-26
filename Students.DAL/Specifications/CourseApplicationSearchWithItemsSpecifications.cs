using Students.DAL.Models;
using Students.DAL.Specifications.Base;

namespace Students.DAL.Specifications
{
    public class CourseApplicationSearchWithItemsSpecifications : BaseSpecification<CourseApplication>
    {
        public CourseApplicationSearchWithItemsSpecifications(string userId,int courseId)
     : base(c => c.Student.UserId == userId && c.CourseId == courseId)
        {
            AddInclude(c => c.Student);
            AddInclude(c => c.Course);
        }
    }
}
