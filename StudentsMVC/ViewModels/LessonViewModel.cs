using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Students.MVC.ViewModels
{
    public class LessonViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Не указано название занятия")]
        [Display(Name = "Название занятия")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указан номер занятия")]
        [Display(Name = "Номер занятия")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Номер занятия должен быть больше 0")]
        public int NumberLesson { get; set; } 
        [Display(Name = "Домашние занятия")]
        [Required(ErrorMessage = "Не указано домашнее задание")]
        public string Homework { get; set; }

        [Display(Name = "Описание занятия")]
        [Required(ErrorMessage = "Не указано описание занятия")]
        public string Description { get; set; }

        [Display(Name = "Курс")]
        [Required(ErrorMessage = "Не указан курс")]
        public int CourseId { get; set; }
        public CourseViewModel Course { get; set; }
        public IEnumerable<CourseViewModel> Courses { get; set; }
        public IEnumerable<AssessmentViewModel> Assessments { get; set; }
        public string ReturnUrl { get; set; }
    }
}
