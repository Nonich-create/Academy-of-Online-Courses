using Students.DAL.Models.Base;
using System;

namespace Students.DAL.Models
{
    public class LessonTimes : BaseModel
    {
        public DateTime? DateLesson { get; set; }
        public int? LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public int? GroupId { get; set; }
        public Group Group { get; set; }
    }
}
