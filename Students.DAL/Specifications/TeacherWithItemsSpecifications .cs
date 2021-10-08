using Students.DAL.Models;
using Students.DAL.Specifications.Base;


namespace Students.DAL.Specifications
{
    public class TeacherWithItemsSpecifications : BaseSpecification<Teacher>
    {
        public TeacherWithItemsSpecifications()
         : base(null)
        {
            AddInclude(t => t.User);
        }
    }
}