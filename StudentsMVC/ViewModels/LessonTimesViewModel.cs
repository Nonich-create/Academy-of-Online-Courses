using Students.MVC.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Students.MVC.ViewModels
{
    public class LessonTimesViewModel : BaseViewModel
    {   
        [Required(ErrorMessage = "Не указана дата проведение занятия")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата проведение занятия")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DateLesson { get; set; }

        public int LessonId { get; set; }
        public LessonViewModel Lesson { get; set; }
        public IEnumerable<LessonViewModel> Lessons { get; set; }

        public int GroupId { get; set; }
        public GroupViewModel Group { get; set; }
        public IEnumerable<GroupViewModel> Groups { get; set; }

    }
}
