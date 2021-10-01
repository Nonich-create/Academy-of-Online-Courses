using System.ComponentModel.DataAnnotations;


namespace Students.DAL.Enum
{
    public enum EnumSearchParametersCourseApplication
    {
        [Display(Name = "Не определён")]
        None = 0,
        [Display(Name = "По курсу")]
        Course_Name = 12,
        [Display(Name = "По фамилии")]
        Student_Surname = 7,
        [Display(Name = "По отчёству")]
        Student_MiddleName = 8,
        [Display(Name = "По имени")]
        Student_Name = 6,
    }
}
 