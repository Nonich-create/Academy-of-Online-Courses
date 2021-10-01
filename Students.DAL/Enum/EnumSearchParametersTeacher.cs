using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Enum
{
    public enum EnumSearchParametersTeacher
    {
        [Display(Name = "Не определён")]
        None = 0,
        [Display(Name = "Имени")]
        Name = 1,
        [Display(Name = "Фамилии")]
        Surname = 2,
        [Display(Name = "Отчеству")]
        MiddleName = 3,
    }
}
