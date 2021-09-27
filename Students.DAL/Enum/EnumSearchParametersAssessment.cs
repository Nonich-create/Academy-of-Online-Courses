﻿using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Enum
{
     public enum EnumSearchParametersAssessment
    {
        [Display(Name = "Не определён")]
        none = 0,
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