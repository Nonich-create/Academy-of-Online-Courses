using Students.DAL.Models;
using Students.DAL.Specifications.Base;


namespace Students.DAL.Specifications
{
    public class ManagerWithItemsSpecifications : BaseSpecification<Manager>
    {
        public ManagerWithItemsSpecifications()
         : base(null)
        {
            AddInclude(m => m.User);
        }
    }
}