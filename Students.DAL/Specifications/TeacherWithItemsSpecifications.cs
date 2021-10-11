using Students.DAL.Enum;
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
        public TeacherWithItemsSpecifications(int currentPage, int pageSize)
: base(null)
        {
            AddInclude(t => t.User);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(s => s.Surname);
        }
        public TeacherWithItemsSpecifications(int currentPage, int pageSize, string stringSearch, EnumSearchParameters searchParametr)
      : base(null)
        {
            AddInclude(t => t.User);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(t => t.Surname);
            ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public TeacherWithItemsSpecifications(string stringSearch, EnumSearchParameters searchParametr)
    : base(null)
        {
            AddInclude(t => t.User);
            ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public TeacherWithItemsSpecifications(string stringSearch)
  : base(null)
        {
            AddInclude(t => t.User);
            ApplyWhere(stringSearch);
        }
    }
}