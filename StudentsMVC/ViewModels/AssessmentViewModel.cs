using Students.MVC.ViewModels.Base;
using System.ComponentModel.DataAnnotations;


namespace Students.MVC.ViewModels
{
    public class AssessmentViewModel : ModelReturnUrl
    {
        public int Id { get; set; }
        [Display(Name = "Студент")]
        [Required(ErrorMessage = "Не указан студент")]
        public int StudentId { get; set; }
        public StudentViewModel Student { get; set; }
        [Display(Name = "Занятия")]
        [Required(ErrorMessage = "Не указано занятие")]
        public int LessonId { get; set; }
        public LessonViewModel Lesson { get; set; }
        [Display(Name = "Оценка")]
        [Range(0, 10, ErrorMessage = "Недопустимая оценка")]
        public int? Score { get; set; }
    }
}
