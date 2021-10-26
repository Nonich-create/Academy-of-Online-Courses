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
            AddInclude(l => l.Group.Course);
        }

        public LessonTimesWithItemsSpecifications(int id)
     : base(l => l.Id == id)
        {
            AddInclude(l => l.Lesson);
            AddInclude(l => l.Group);
            AddInclude(l => l.Group.Course);
        }

        public LessonTimesWithItemsSpecifications(uint groupId)
  : base(l => l.GroupId == groupId)
        {
        }

        public LessonTimesWithItemsSpecifications(int currentPage, int pageSize)
: base(null)
        {
            AddInclude(l => l.Lesson);
            AddInclude(l => l.Group);
            AddInclude(l => l.Group.Course);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(l => l.Group.NumberGroup);
        }

        public LessonTimesWithItemsSpecifications(int groupId, int currentPage, int pageSize)
: base(l => l.GroupId == groupId)
        {
            AddInclude(l => l.Lesson);
            AddInclude(l => l.Group);
            AddInclude(l => l.Group.Course);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(l => l.Lesson.NumberLesson);
        }

        public LessonTimesWithItemsSpecifications(int currentPage, int pageSize, string stringSearch, EnumSearchParameters searchParametr)
: base(null)
        {
            AddInclude(l => l.Lesson);
            AddInclude(l => l.Group);
            AddInclude(l => l.Group.Course);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(l => l.Group.NumberGroup);
            if (!string.IsNullOrEmpty(stringSearch) && searchParametr != EnumSearchParameters.None)
                ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public LessonTimesWithItemsSpecifications(string stringSearch, EnumSearchParameters searchParametr)
    : base(null)
        {
            ApplyOrderBy(l => l.Group.NumberGroup);
            if (!string.IsNullOrEmpty(stringSearch) && searchParametr != EnumSearchParameters.None)
            {
                ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
            }
        }

        public LessonTimesWithItemsSpecifications(int groupId , string stringSearch, EnumSearchParameters searchParametr)
 : base(l => l.GroupId == groupId)
        {
            ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public LessonTimesWithItemsSpecifications(string stringSearch)
    : base(null)
        {
            AddInclude(l => l.Lesson);
            AddInclude(l => l.Group);
            AddInclude(l => l.Group.Course);
            ApplyOrderBy(l => l.Group.NumberGroup);
            ApplyWhere(stringSearch);
        }
    }
}