using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Enum
{
    public enum EnumSearchParametersManager
    {
        [Display(Name = "Не определён")]
        none = 0,
        [Display(Name = "Имени")]
        Name = 1,
        [Display(Name = "Фамилии")]
        Surname = 2,
        [Display(Name = "Отчеству")]
        MiddleName = 3,
    }
}
