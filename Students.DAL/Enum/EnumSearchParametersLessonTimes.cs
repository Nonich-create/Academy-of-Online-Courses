using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Enum
{
    public enum EnumSearchParametersLessonTimes
    {
        [Display(Name = "Не определён")]
        none = 0,
        [Display(Name = "По уроку")]
        Lesson_Name = 9,
        [Display(Name = "По номеру группы")]
        Group_NumberGroup = 4,
        [Display(Name = "По дате проведение занятия")]
        DateOfTheLesson = 12,
    }
}
 