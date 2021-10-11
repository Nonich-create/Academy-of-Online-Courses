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
        public ManagerWithItemsSpecifications(int currentPage, int pageSize)
: base(null)
        {
            AddInclude(m => m.User);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(s => s.Surname);
        }
    }
}