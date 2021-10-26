using Students.DAL.Enum;
using Students.DAL.Models;
using Students.DAL.Specifications.Base;


namespace Students.DAL.Specifications
{
    public class AssessmentWithItemsSpecifications : BaseSpecification<Assessment>
    {
        public AssessmentWithItemsSpecifications()
         : base(null)
        {
            AddInclude(a => a.Lesson);
            AddInclude(a => a.Student);
            ApplyOrderBy(a => a.Lesson.NumberLesson);
        }
        public AssessmentWithItemsSpecifications(int currentPage, int pageSize)
: base(null)
        {
            AddInclude(a => a.Lesson);
            AddInclude(a => a.Student);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(a => a.Lesson.NumberLesson);
        }
        public AssessmentWithItemsSpecifications(int currentPage, int pageSize, string stringSearch, EnumSearchParameters searchParametr)
: base(null)
        {
            AddInclude(a => a.Lesson);
            AddInclude(a => a.Student);
            ApplyPaging((currentPage - 1) * pageSize, pageSize);
            ApplyOrderBy(a => a.Lesson.NumberLesson);
            if (!string.IsNullOrEmpty(stringSearch) && searchParametr != EnumSearchParameters.None)
                ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public AssessmentWithItemsSpecifications(string stringSearch, EnumSearchParameters searchParametr)
    : base(null)
        {
            AddInclude(a => a.Lesson);
            AddInclude(a => a.Student);
            ApplyOrderBy(a => a.Lesson.NumberLesson);
            if (!string.IsNullOrEmpty(stringSearch) && searchParametr != EnumSearchParameters.None)
                ApplyWhere($"{searchParametr.ToString().Replace('_', '.')}.Contains(\"{stringSearch}\")");
        }

        public AssessmentWithItemsSpecifications(string stringSearch)
    : base(null)
        {
            AddInclude(a => a.Lesson);
            AddInclude(a => a.Lesson.Course);
            AddInclude(a => a.Student);
            ApplyOrderBy(a => a.Lesson.NumberLesson);
            ApplyWhere(stringSearch);
        }

        public AssessmentWithItemsSpecifications(int Id)
  : base(a => a.Id == Id)
        {
            AddInclude(a => a.Lesson);
            AddInclude(a => a.Lesson.Course);
            AddInclude(a => a.Student);
            ApplyOrderBy(a => a.Lesson.NumberLesson);
        }

        public AssessmentWithItemsSpecifications(uint studentId, int courseId)
: base(a => a.StudentId == studentId || a.Lesson.CourseId == courseId)
        {
            AddInclude(a => a.Lesson);
            AddInclude(a => a.Lesson.Course);
            AddInclude(a => a.Student);
            ApplyOrderBy(a => a.Lesson.NumberLesson);
        }
    }
}