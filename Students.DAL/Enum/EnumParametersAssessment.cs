using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Enum
{
     public enum EnumParametersAssessment
    {
        [Display(Name = "Не определён")]
        None = 0,
        [Display(Name = "По имени")]
        Student_Name = 6,
        [Display(Name = "По фамилии")]
        Student_Surname = 7,
        [Display(Name = "По отчёству")]
        Student_MiddleName = 8,
        [Display(Name = "По уроку")]
        Lesson_Name = 9,
    }
}
