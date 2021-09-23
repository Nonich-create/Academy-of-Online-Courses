﻿using System.ComponentModel.DataAnnotations;


namespace Students.DAL.Enum
{
    public enum EnumSearchParametersStudent
    {
        [Display(Name = "Не определён")]
        none = 0,
        [Display(Name = "По имени")]
        Name = 1,
        [Display(Name = "По фамилии")]
        Surname = 2,
        [Display(Name = "По отчеству")]
        MiddleName = 3,
        [Display(Name = "По группе")]
        Group = 4,
        [Display(Name = "По курсу")]
        Course = 5,
    }
}
