using Students.DAL.Models;
using Students.DAL.Specifications.Base;


namespace Students.DAL.Specifications
{
    public class StudentWithItemsSpecifications : BaseSpecification<Student>
    {
        public StudentWithItemsSpecifications()
         : base(null)
        {
            AddInclude(s => s.User);
            AddInclude(s => s.Group);
            AddInclude(s => s.Group.Course);
        }
    }
}