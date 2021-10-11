using Students.DAL.Enum;
using Students.DAL.Models;
using Students.DAL.Specifications.Base;


namespace Students.DAL.Specifications
{
    public class LessonTimesWithItemsSpecifications : BaseSpecification<LessonTimes>
    {
        public LessonTimesWithItemsSpecifications()
         : base(null)
        {
            AddInclude(l => l.Lesson);
            AddInclude(l => l.Group);
        }

        public LessonTimesWithItemsSpecifications(int currentPage, int pageSize)
: base(null)
        {
            AddInclude(l => l.Lesson);
            AddInclude(l => l.Group);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(l => l.Group.NumberGroup);
        }
        public LessonTimesWithItemsSpecifications(int currentPage, int pageSize, string stringSearch, EnumSearchParameters searchParametr)
: base(null)
        {
            AddInclude(l => l.Lesson);
            AddInclude(l => l.Group);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(l => l.Group.NumberGroup);
            ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public LessonTimesWithItemsSpecifications(string stringSearch, EnumSearchParameters searchParametr)
    : base(null)
        {
            AddInclude(l => l.Lesson);
            AddInclude(l => l.Group);
            ApplyOrderBy(l => l.Group.NumberGroup);
            ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public LessonTimesWithItemsSpecifications(string stringSearch)
    : base(null)
        {
            AddInclude(l => l.Lesson);
            AddInclude(l => l.Group);
            ApplyOrderBy(l => l.Group.NumberGroup);
            ApplyWhere(stringSearch);
        }
    }
}