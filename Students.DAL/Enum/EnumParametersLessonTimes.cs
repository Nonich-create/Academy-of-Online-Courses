using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Enum
{
    public enum EnumParametersLessonTimes
    {
        [Display(Name = "Не определён")]
        None = 0,
        [Display(Name = "По занятию")]
        Lesson_Name = 9,
        [Display(Name = "По номеру группы")]
        Group_NumberGroup = 4,
    }
}
 