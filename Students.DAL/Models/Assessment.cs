using Students.DAL.Models.Base;

namespace Students.DAL.Models
{
    public class Assessment : BaseModel
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public int? Score { get; set; }
    }
}
