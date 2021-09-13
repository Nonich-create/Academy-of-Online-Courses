using System;
using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Models
{
    public class LessonPlan
    {
        [Key]
        public int LessonPlanId { get; set; }
        public DateTime DateOfTheLesson { get; set; }
        public int? LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public int? GroupId { get; set; }
        public Group Group { get; set; }
    }
}
