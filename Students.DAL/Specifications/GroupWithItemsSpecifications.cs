using Students.DAL.Enum;
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
        public GroupWithItemsSpecifications(int currentPage, int pageSize)
: base(null)
        {
            AddInclude(g => g.Course);
            AddInclude(g => g.Teacher);
            AddInclude(g => g.Manager);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(g => g.NumberGroup);
        }
        public GroupWithItemsSpecifications(int teacherId, int currentPage, int pageSize)
: base(t => t.TeacherId == teacherId)
        {
            AddInclude(g => g.Course);
            AddInclude(g => g.Teacher);
            AddInclude(g => g.Manager);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(g => g.NumberGroup);
        }

        public GroupWithItemsSpecifications(int teacherId)
: base(t => t.TeacherId == teacherId)
        {
        }

        public GroupWithItemsSpecifications(int currentPage, int pageSize, string stringSearch, EnumSearchParameters searchParametr)
: base(null)
        {   
            AddInclude(g => g.Course);
            AddInclude(g => g.Teacher);
            AddInclude(g => g.Manager);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(g => g.NumberGroup);
            ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public GroupWithItemsSpecifications(string stringSearch, EnumSearchParameters searchParametr)
    : base(null)
        {
            AddInclude(g => g.Course);
            AddInclude(g => g.Teacher);
            AddInclude(g => g.Manager);
            ApplyOrderBy(g => g.NumberGroup);
            ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public GroupWithItemsSpecifications(string stringSearch)
    : base(null)
        {
            AddInclude(g => g.Course);
            AddInclude(g => g.Teacher);
            AddInclude(g => g.Manager);
            ApplyOrderBy(g => g.NumberGroup);
            ApplyWhere(stringSearch);
        }

        public GroupWithItemsSpecifications(int courseId, GroupStatus groupStatus)
: base(g => g.CourseId == courseId && g.GroupStatus == groupStatus)
        {
            AddInclude(g => g.Course);
            AddInclude(g => g.Teacher);
            AddInclude(g => g.Manager);
        }

        public GroupWithItemsSpecifications( uint groupId, GroupStatus groupStatus)
: base(g => g.Id == groupId && g.GroupStatus == groupStatus)
        {
            AddInclude(g => g.Course);
            AddInclude(g => g.Teacher);
            AddInclude(g => g.Manager);
        }
    }   
}