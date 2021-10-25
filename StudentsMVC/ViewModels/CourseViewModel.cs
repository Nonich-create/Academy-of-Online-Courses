using Students.MVC.ViewModels.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Students.MVC.ViewModels
{
    public class CourseViewModel: BaseViewModel
    {
        [Required(ErrorMessage = "Не указано название курса")]
        [Display(Name = "Название курсов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указано описание курса")]
        [Display(Name = "Описание курса")]
        public string Description { get; set; }

 
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)"),Required(ErrorMessage ="Не правильно указана цена")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Не указана длительность курса")]
        [Display(Name = "Длительность курса в днях")]
        public uint Duration { get; set; }

        [Display(Name = "Изображения")]
        public string URLImagePhoto { get; set; }

        [Display(Name = "Отображения курса на витрине")]
        public bool Visible { get; set; } = false;

        public IEnumerable<LessonViewModel> Lessons { get; set; }
        public IEnumerable<GroupViewModel> Groups { get; set; }
    }
}
