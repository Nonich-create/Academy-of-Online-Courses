using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Students.MVC.ViewModels
{
    public class EditStudentViewModel
    {
        public int StudentId { get; set; }
        [Display(Name = "Имя")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Не указана фамилия")]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Не указано отчество")]
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }
        [UIHint("Url")]
        [Display(Name = "URL")]
        public string URLImagePhoto { get; set; }
        [Display(Name = "Номер группы")]
        public int? GroupId { get; set; }
        public GroupViewModel Group { get; set; }
        public IEnumerable<GroupViewModel> Groups { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        public DateTime DateOfBirth { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
