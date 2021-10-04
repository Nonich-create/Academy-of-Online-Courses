using System;
using System.ComponentModel.DataAnnotations;

namespace Students.MVC.ViewModels
{
    public class EditStudentViewModel: PersonEditViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Номер группы")]
        public int? GroupId { get; set; }
    }
}
