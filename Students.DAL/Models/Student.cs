using System;
using System.Collections.Generic;

namespace Students.DAL.Models
{
    public class Student : Person
    {
        public DateTime DateOfBirth { get; set; }
        public int? GroupId { get; set; }
        public Group Group { get; set; }
        public List<Assessment> Assessments { get; set; }
        public List<CourseApplication> CourseApplication { get; set; }
    }
}
