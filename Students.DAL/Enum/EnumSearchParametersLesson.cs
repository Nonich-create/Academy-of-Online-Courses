using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Enum
{
    public enum EnumSearchParametersLesson
    {
        [Display(Name = "Не определён")]
        none = 0,
        [Display(Name = "По уроку")]
        Name = 1,
        [Display(Name = "По курсу")]
        Course_Name = 12,
    }

}
