using System.ComponentModel.DataAnnotations;
using Students.DAL.Enum;

namespace Students.DAL.Models
{
    public class CourseApplication
    {
        [Key]
        public int Id { get; set; }
        public EnumApplicationStatus ApplicationStatus { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
