using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Students.DAL.Enum;

namespace Students.MVC.ViewModels
{
    public class StudentViewModel: PersonViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Номер группы")]
        public int? GroupId { get; set; }
        public GroupViewModel Group { get; set; }
        public IEnumerable<AssessmentViewModel> Assessments { get; set; }
    }
}
