using Students.DAL.Models;
using Students.DAL.Specifications.Base;


namespace Students.DAL.Specifications
{
    public class GroupWithItemsSpecifications : BaseSpecification<Group>
    {
        public GroupWithItemsSpecifications()
         : base(null)
        {
            AddInclude(g => g.Course);
            AddInclude(g => g.Teacher);
            AddInclude(g => g.Manager);
        }
    }
}