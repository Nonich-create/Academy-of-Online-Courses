using System.ComponentModel.DataAnnotations;


namespace Students.MVC.ViewModels
{
    public class ApplicationCourseViewModel
    {
        [Display(Name = "№")]
        public int ApplicationCourseId { get; set; }
        [Display(Name = "Статус заявки")]
        public string ApplicationStatus { get; set; }
        [Display(Name = "Курс")]
        [Required(ErrorMessage = "Не указан курc")]
        public int CourseId { get; set; }
        public CourseViewModel Course { get; set; }
        [Display(Name = "ФИО")]
        public int StudentId { get; set; }
        public StudentViewModel Student { get; set; }
    }
}
