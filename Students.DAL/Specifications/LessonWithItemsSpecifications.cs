using Students.DAL.Enum;
using Students.DAL.Models;
using Students.DAL.Specifications.Base;


namespace Students.DAL.Specifications
{
    public class LessonWithItemsSpecifications : BaseSpecification<Lesson>
    {
        public LessonWithItemsSpecifications()
         : base(null)
        {
            AddInclude(l => l.Course);
        }
        public LessonWithItemsSpecifications(int courseId)
    : base(l => l.Course.Id == courseId)
        {
            AddInclude(l => l.Course);
            ApplyOrderBy(l => l.NumberLesson);
        }
        public LessonWithItemsSpecifications(uint lessonId)
: base(l => l.Id == lessonId)
        {
            AddInclude(l => l.Course);
        }
        public LessonWithItemsSpecifications(int currentPage, int pageSize)
: base(null)
        {
            AddInclude(l => l.Course);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(l => l.Course.Name);
        }
        public LessonWithItemsSpecifications(int currentPage, int pageSize, string stringSearch, EnumSearchParameters searchParametr)
: base(null)
        {
            AddInclude(l => l.Course);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(l => l.Course.Name);
            if (!string.IsNullOrEmpty(stringSearch) && searchParametr != EnumSearchParameters.None)
                ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public LessonWithItemsSpecifications(string stringSearch, EnumSearchParameters searchParametr)
    : base(null)
        {
            AddInclude(l => l.Course);
            ApplyOrderBy(l => l.Course.Name);
            if (!string.IsNullOrEmpty(stringSearch) && searchParametr != EnumSearchParameters.None)
                ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public LessonWithItemsSpecifications(int courseId, string stringSearch, EnumSearchParameters searchParametr)
: base(l => l.CourseId == courseId)
        {
            AddInclude(l => l.Course);
            ApplyOrderBy(l => l.NumberLesson);
            ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public LessonWithItemsSpecifications(string stringSearch)
  : base(null)
        {
            AddInclude(l => l.Course);
            ApplyOrderBy(l => l.Course.Id);
            ApplyWhere(stringSearch);
        }

        public LessonWithItemsSpecifications(int currentPage, int pageSize, string stringSearch, int courseId, EnumSearchParameters searchParametr)
: base(l => l.CourseId == courseId)
        {
            AddInclude(l => l.Course);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(l => l.NumberLesson);
            if (!string.IsNullOrEmpty(stringSearch) && searchParametr != EnumSearchParameters.None)
                ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }
        public LessonWithItemsSpecifications(int currentPage, int pageSize, int courseId)
: base(l => l.CourseId == courseId)
        {
            AddInclude(l => l.Course);
            ApplyOrderBy(l => l.NumberLesson);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
        }
    }
}