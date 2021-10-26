using Students.DAL.Enum;
using Students.DAL.Models;
using Students.DAL.Specifications.Base;

namespace Students.DAL.Specifications
{
    public class UserWithItemsSpecifications : BaseSpecification<ApplicationUser>
    {
        public UserWithItemsSpecifications()
         : base(null)
        {
       
        }
        public UserWithItemsSpecifications(int currentPage, int pageSize)
      : base(null)
        {
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(u => u.Email);
        }
        public UserWithItemsSpecifications(int currentPage, int pageSize, string stringSearch, EnumSearchParameters searchParametr)
        : base(null)
        {;
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(u => u.Email);
            if (!string.IsNullOrEmpty(stringSearch) && searchParametr != EnumSearchParameters.None)
                ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public UserWithItemsSpecifications(string stringSearch, EnumSearchParameters searchParametr)
    : base(null)
        {
            ApplyOrderBy(u => u.Email);
            if (!string.IsNullOrEmpty(stringSearch) && searchParametr != EnumSearchParameters.None)
                ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
            
        }
        public UserWithItemsSpecifications(string stringSearch)
: base(null)
        {
            ApplyOrderBy(u => u.Email);
            ApplyWhere(stringSearch);
        }

    }
}