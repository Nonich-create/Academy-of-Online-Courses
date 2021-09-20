using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Students.MVC.ViewModels
{
    public class StudentViewModel: PersonViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Номер группы")]
        public int? GroupId { get; set; }
        public GroupViewModel Group { get; set; }
        public List<AssessmentViewModel> Assessments { get; set; }
        public List<StudentViewModel> StudentViewModels { get; set; }

    }
}
