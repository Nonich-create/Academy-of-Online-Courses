using System.ComponentModel.DataAnnotations;


namespace Students.MVC.ViewModels
{
    public class AssessmentViewModel
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
        public int? Score { get; set; }

    }
}
