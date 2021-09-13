using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Models
{
    public class Assessment
    {
        [Key]
        public int AssessmentId { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public int? Score { get; set; }
    }
}
