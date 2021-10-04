using System.ComponentModel.DataAnnotations;


namespace Students.DAL.Enum
{
    public enum EnumParametersGroup
    {
        [Display(Name = "Не определён")]
        None = 0,
        [Display(Name = "По номеру группы")]
        NumberGroup = 13,
        [Display(Name = "По курсу")]
        Course_Name = 12,
        [Display(Name = "По фамилии менеджера")]
        Manager_Surname = 16,
        [Display(Name = "По фамилии преподователя")]
        Teacher_Surname = 17,
    }
 
}
