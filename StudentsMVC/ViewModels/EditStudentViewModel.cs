using System;
using System.ComponentModel.DataAnnotations;


namespace Students.MVC.ViewModels
{
    public class EditStudentViewModel : EditPersonViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Номер группы")]
        public int? GroupId { get; set; }
        public GroupViewModel Group { get; set; }
    }
}
