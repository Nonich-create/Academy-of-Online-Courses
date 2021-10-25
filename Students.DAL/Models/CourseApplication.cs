using System;
using System.ComponentModel.DataAnnotations;
using Students.DAL.Models.Base;

namespace Students.DAL.Models
{
    public class CourseApplication : BaseModel
    {
        public ApplicationStatus ApplicationStatus { get; set; }
        public DateTime ApplicationDate { get; set; } = DateTime.Now;
        public DateTime UpdateDate { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }

    public enum ApplicationStatus
    {
        [Display(Name = "Открыта")]
        Open,
        [Display(Name = "Отменена")]
        Cancelled,
        [Display(Name = "Закрыта")]
        Close,
    }
}
 