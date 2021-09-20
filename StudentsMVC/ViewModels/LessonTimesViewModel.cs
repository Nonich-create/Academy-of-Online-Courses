using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Students.MVC.ViewModels
{
    public class LessonTimesViewModel
    {
      
        public int Id { get; set; }

        [Required(ErrorMessage = "Не указана дата проведение занятия")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата проведение занятия")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfTheLesson { get; set; }

        public int LessonId { get; set; }
        public LessonViewModel Lesson { get; set; }
        public List<LessonViewModel> Lessons { get; set; }

        public int GroupId { get; set; }
        public GroupViewModel Group { get; set; }
        public List<GroupViewModel> Groups { get; set; }

    }
}
