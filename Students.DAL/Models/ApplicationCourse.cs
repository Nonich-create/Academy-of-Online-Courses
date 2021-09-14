using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Students.DAL.Models
{
    public class ApplicationCourse
    {
        [Key]
        public int ApplicationCourseId { get; set; }
        public string ApplicationStatus { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public DateTime Created { get; set; }
    }
}
