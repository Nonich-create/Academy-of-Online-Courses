using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Students.DAL.Models.Base;

namespace Students.DAL.Models
{
    public class Group : BaseModel
    {
        public string NumberGroup { get; set; }
        public DateTime DateStart { get; set; }
        public GroupStatus GroupStatus { get; set; }
        public int CountMax { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public int ManagerId { get; set; }
        public Manager Manager { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
     }

    public enum  GroupStatus
    {
        [Display(Name = "Набор")]
        Set,
        [Display(Name = "Закрыта")]
        Close,
        [Display(Name = "Отменена")]
        Cancelled,
        [Display(Name = "Обучение")]
        Training
    }
}
