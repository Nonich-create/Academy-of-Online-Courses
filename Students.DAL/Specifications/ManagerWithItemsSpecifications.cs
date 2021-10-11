using Students.DAL.Enum;
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
        public ManagerWithItemsSpecifications(int currentPage, int pageSize, string stringSearch, EnumSearchParameters searchParametr)
: base(null)
        {
            AddInclude(m => m.User);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(t => t.Surname);
            ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public ManagerWithItemsSpecifications(string stringSearch, EnumSearchParameters searchParametr)
    : base(null)
        {
            AddInclude(m => m.User);
            ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public ManagerWithItemsSpecifications(string stringSearch)
  : base(null)
        {
            AddInclude(m => m.User);
            ApplyWhere(stringSearch);
        }
    }
}
