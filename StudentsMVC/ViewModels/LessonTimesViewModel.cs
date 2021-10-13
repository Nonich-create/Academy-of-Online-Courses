using Students.DAL.Models;
using Students.MVC.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Students.MVC.ViewModels
{
    public class LessonTimesViewModel : ModelReturnUrl
    {
      
        public int Id { get; set; }

        [Required(ErrorMessage = "Не указана дата проведение занятия")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата проведение занятия")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfTheLesson { get; set; }

        public int LessonId { get; set; }
        public LessonViewModel Lesson { get; set; }
        public IEnumerable<LessonViewModel> Lessons { get; set; }

        public int GroupId { get; set; }
        public GroupViewModel Group { get; set; }
        public IEnumerable<GroupViewModel> Groups { get; set; }

    }
}
