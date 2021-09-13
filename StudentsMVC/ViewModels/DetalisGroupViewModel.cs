using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Students.MVC.ViewModels
{
    public class DetalisGroupViewModel
    {
        public int GroupId { get; set; }

        [Required(ErrorMessage = "Не указан номер группы")]
        [Display(Name = "Номер группы")]
        public string NumberGroup { get; set; }

        [Required(ErrorMessage = "Не указана дата старта группы")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата старта группы")]
        public DateTime DateStart { get; set; }

        [Display(Name = "Статус группы")]
        public string GroupStatus { get; set; }

        [Required(ErrorMessage = "Не указана максимальная количество студентов")]
        [Display(Name = "Количество студентов")]
        public int CountMax { get; set; }

        [Required(ErrorMessage = "Не указан менеджер")]
        [Display(Name = "Менеджер")]
        public int ManagerId { get; set; }
        public ManagerViewModel Manager { get; set; }
        public IEnumerable<ManagerViewModel> Manageres { get; set; }

        [Required(ErrorMessage = "Не указан преподователь")]
        [Display(Name = "Преподователь")]
        public int TeacherId { get; set; }
        public TeacherViewModel Teacher { get; set; }
        public IEnumerable<TeacherViewModel> Teachers { get; set; }

        [Required(ErrorMessage = "Не указан курс")]
        [Display(Name = "Курс")]
        public int CourseId { get; set; }
        public CourseViewModel Course { get; set; }
        public IEnumerable<CourseViewModel> Courses { get; set; }
        public List<StudentViewModel> StudentsViewModels { get; set; }



    }
}
