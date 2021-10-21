using System.ComponentModel.DataAnnotations;
using Students.DAL.Models;
using Students.MVC.ViewModels.Base;

namespace Students.MVC.ViewModels
{
    public class CourseApplicationViewModel : BaseViewModel
    {
        [Display(Name = "Статус заявки")]
        public ApplicationStatus ApplicationStatus { get; set; }
        [Display(Name = "Курс")]
        [Required(ErrorMessage = "Не указан курc")]
        public int CourseId { get; set; }
        public CourseViewModel Course { get; set; }
        [Display(Name = "ФИО")]
        public int StudentId { get; set; }
        public StudentViewModel Student { get; set; }
    }
}
