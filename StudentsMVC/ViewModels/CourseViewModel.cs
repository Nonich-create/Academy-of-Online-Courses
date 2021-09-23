using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Students.MVC.ViewModels
{
    public class CourseViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Не указано название курса")]
        [Display(Name = "Название курсов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указано описание курса")]
        [Display(Name = "Описание курса")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Не указана цена курса")]
        [DataType(DataType.Currency)]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Не указана длительность курса")]
        [Display(Name = "Длительность курса")]
        public string Duration { get; set; }

        [Display(Name = "Изображения")]
        public string URLImagePhoto { get; set; }

        [Display(Name = "Отображения курса на витрине")]
        public bool Visible { get; set; } = false;

        public IEnumerable<LessonViewModel> Lessons { get; set; }
        public IEnumerable<GroupViewModel> Groups { get; set; }
    }
}
