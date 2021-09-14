using System.ComponentModel.DataAnnotations;
using Students.DAL.Enum;

namespace Students.MVC.ViewModels
{
    public class CourseApplicationViewModel
    {
        [Display(Name = "№")]
        public int Id { get; set; }
        [Display(Name = "Статус заявки")]
        public EnumApplicationStatus ApplicationStatus { get; set; }
        [Display(Name = "Курс")]
        [Required(ErrorMessage = "Не указан курc")]
        public int CourseId { get; set; }
        public CourseViewModel Course { get; set; }
        [Display(Name = "ФИО")]
        public int StudentId { get; set; }
        public StudentViewModel Student { get; set; }
    }
}
