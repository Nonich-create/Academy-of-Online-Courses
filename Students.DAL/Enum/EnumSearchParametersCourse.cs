using System.ComponentModel.DataAnnotations;


namespace Students.DAL.Enum
{
    public enum EnumSearchParametersCourse
    {
        [Display(Name = "Не определён")]
        none = 0,
        [Display(Name = "По курсу")]
        Name = 1,
    }
}
